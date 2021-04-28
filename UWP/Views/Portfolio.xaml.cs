using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using UWP.Shared.Models;
using UWP.UserControls;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using static UWP.APIs.CryptoCompare;

namespace UWP.Views {

    public partial class Portfolio : Page {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        private string PortfolioKey = "Portfolio";

        private int EditingPurchaseId { get; set; }

        private bool ShowingDetails = false;

        public Portfolio() {
            this.InitializeComponent();   
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            vm.Portfolio = await RetrievePortfolio();
            UpdatePortfolio();
        }

        /// ###############################################################################################
        /// Get portfolio from LocalStorage
        internal async Task<ObservableCollection<PurchaseModel>> RetrievePortfolio() {
            ObservableCollection<PurchaseModel> portfolio;

            var portfolioV2 = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio);

            if (portfolioV2.Count > 0)
                return new ObservableCollection<PurchaseModel>(portfolioV2);

            /// If it is empty, there might be an old portfolio in the old format and key
            var portfolioV1 = await LocalStorageHelper.ReadObject<ObservableCollection<PurchaseClass>>("portfolio");
            if (portfolioV1.Count < 0)
                return new ObservableCollection<PurchaseModel>();

            /// For retrocompatibility with old portfolios
            portfolio = new ObservableCollection<PurchaseModel>();
            foreach (var p in portfolioV1) {
                portfolio.Add(new PurchaseModel() {
                    Crypto = p.Crypto,
                    CryptoLogo = p.CryptoLogo,
                    CryptoQty = p.CryptoQty,
                    Currency = p.c,
                    Date = p.Date,
                    Exchange = p.Exchange,
                    InvestedQty = p.InvestedQty
                });
            }
            return portfolio;
        }


        /// ###############################################################################################
        ///  For sync all
        internal async void UpdatePortfolio() {
            /// Empty diversification chart and reset the Total amounts
            PortfolioChartGrid.ColumnDefinitions.Clear();
            PortfolioChartGrid.Children.Clear();
            vm.TotalInvested = 0;
            vm.TotalWorth = 0;

            /// Update the purchase details first
            for (int i = 0; i < vm.Portfolio.Count; i++)
                await PortfolioHelper.UpdatePurchase(vm.Portfolio[i]);

            /// Create the diversification grid
            var grouped = vm.Portfolio.GroupBy(x => x.Crypto);
            foreach (var purchases in grouped) {
                var crypto = purchases.Key.ToUpperInvariant();
                var worth = purchases.ToList().Sum(x => x.InvestedQty);

                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(worth, GridUnitType.Star);
                PortfolioChartGrid.ColumnDefinitions.Add(col);

                var s = new StackPanel();
                s.BorderThickness = new Thickness(0);
                s.Margin = new Thickness(1, 0, 1, 0);
                var t = new TextBlock() {
                    Text = crypto,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 7, 0, 7)
                };
                s.Children.Add(t);
                s.Background = ColorConstants.GetBrush(crypto + "_color", 100);

                PortfolioChartGrid.Children.Add(s);
                Grid.SetColumn(s, PortfolioChartGrid.Children.Count - 1);
            }

            vm.TotalInvested = vm.Portfolio.Sum(x => x.InvestedQty);
            vm.TotalWorth = vm.Portfolio.Sum(x => x.Worth);

            /// Finally, update the chart of the portfolio's worth
            await UpdatePortfolioChart();
        }

        /// ###############################################################################################
        /// Update the chart of the historic values of the portfolio
        private async Task UpdatePortfolioChart() {
            var nPurchases = vm.Portfolio.Count;
            if (nPurchases == 0)
                return;

            /// Optimize by only getting historic for different cryptos
            var uniqueCryptos = new HashSet<string>(vm.Portfolio.Select(x => x.Crypto)).ToList();

            /// Get historic for each unique crypto, get invested qty, and multiply
            /// to get the worth of each crypto to the user's wallet
            var cryptoQties = new List<double>();
            var cryptoWorth = new List<List<double>>();
            var histos = new List<List<HistoricPrice>>(nPurchases);
            foreach (var crypto in uniqueCryptos) {
                var histo = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric_(crypto, timeUnit, limit, aggregate);
                var cryptoQty = vm.Portfolio.Where(x => x.Crypto == crypto).Sum(x => x.CryptoQty);
                cryptoWorth.Add(histo.Select(x => x.Average * cryptoQty).ToList());
                histos.Add(histo);
            }

            /// There might be young cryptos that didnt exist in the past, so take the common minimum 
            var minCommon = histos.Min(x => x.Count);
            if (minCommon == 1)
                return;

            /// Check if all arrays are equal length, if not, remove the leading values
            var sameLength = cryptoWorth.All(x => x.Count == cryptoWorth[0].Count);
            if (!sameLength)
                for (int i = 0; i < histos.Count; i++)
                    histos[i] = histos[i].Skip(Math.Max(0, histos[i].Count() - minCommon)).ToList();


            var worth_arr = new List<double>();
            var dates_arr = histos[0].Select(kk => kk.DateTime).ToList();
            for (int i = 0; i < minCommon; i++)
                worth_arr.Add(cryptoWorth.Select(x => x[i]).Sum());

            /// Create List for chart
            var chartData = new List<ChartPoint>();
            for (int i = 0; i < minCommon; i++) {
                chartData.Add(new ChartPoint {
                    Date = dates_arr.ElementAt(i),
                    Value = (float)worth_arr[i],
                    Volume = 0
                });
            }
            vm.Chart.ChartData = chartData;
            var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
            vm.Chart.LabelFormat = temp.LabelFormat;
            vm.Chart.Minimum = temp.Minimum;
            vm.Chart.MajorStepUnit = temp.MajorStepUnit;
            vm.Chart.MajorStep = temp.MajorStep;
            vm.Chart.TickInterval = temp.TickInterval;

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max);
            vm.Chart.VolumeMax = GraphHelper.GetMaxOfVolume(chartData);
            vm.Chart.VolumeMax = (vm.Chart.VolumeMax == 0) ? 10 : vm.Chart.VolumeMax;
        }

        /// ###############################################################################################
        /// A purchase's right click ContextFlyout
        private void GoToCoinPortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseModel;
            this.Frame.Navigate(typeof(CoinDetails), item.Crypto);
        }

        private void DuplicatePurchase_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;
            var i = vm.Portfolio.IndexOf(purchase);
            var newPurchase = new PurchaseModel() {
                Crypto = purchase.Crypto,
                CryptoName = purchase.CryptoName,
                CryptoLogo = purchase.CryptoLogo,
                CryptoQty = purchase.CryptoQty,
                Currency = purchase.Currency,
                CurrencySymbol = purchase.CurrencySymbol,
                Id = Guid.NewGuid().ToString("N"),
                Type = purchase.Type,
                InvestedQty = purchase.InvestedQty,
                Date = purchase.Date,
                Exchange = purchase.Exchange,
                Notes = purchase.Notes
            };
            vm.Portfolio.Insert(i, newPurchase);
            /// Update the page and save the new portfolio
            UpdatePortfolio();
            LocalStorageHelper.SaveObject(PortfolioKey, vm.Portfolio);
        }

        private void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseModel;
            var items = Portfolio_dg.ItemsSource.Cast<PurchaseModel>().ToList();
            var index = items.IndexOf(item);
            vm.Portfolio.RemoveAt(index);
            /// Update the page and save the new portfolio
            UpdatePortfolio();
            LocalStorageHelper.SaveObject(PortfolioKey, vm.Portfolio);
        }


        /// ###############################################################################################
        private void ToggleDetails_click(object sender, RoutedEventArgs e) {
            ShowingDetails = !ShowingDetails;
            if (ShowingDetails) {
                Portfolio_dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Visible;
                PortfolioChart.Visibility = Visibility.Collapsed;
                TimerangeRadioButtons.Visibility = Visibility.Collapsed;
            }
            else {
                Portfolio_dg.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                PortfolioChart.Visibility = Visibility.Visible;
                TimerangeRadioButtons.Visibility = Visibility.Visible;
            }
        }

        /// ###############################################################################################
        /// Add/Edit purchase dialog
        private void AddPurchase_click(object sender, RoutedEventArgs e) {
            vm.NewPurchase = new PurchaseModel();
            PurchaseDialog.Title = "💵 New purchase";
            PurchaseDialog.PrimaryButtonText = "Add";
            PurchaseDialog.ShowAsync();
        }

        private void EditPurchase_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;
            EditingPurchaseId = vm.Portfolio.IndexOf(purchase);
            vm.NewPurchase = purchase;
            PurchaseDialog.Title = "💵 Edit purchase";
            PurchaseDialog.PrimaryButtonText = "Save";
            PurchaseDialog.ShowAsync();
        }

        private void PurchaseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(vm.NewPurchase.Crypto) || vm.NewPurchase.CryptoQty <= 0 || vm.NewPurchase.InvestedQty < 0) {
                args.Cancel = true;
                new MessageDialog("You must fill Crypto, Amount and Invested fields.").ShowAsync();
            }
            else {
                if (sender.PrimaryButtonText == "Add")
                    vm.Portfolio.Add(vm.NewPurchase);
                else if(sender.PrimaryButtonText == "Save") {
                    vm.Portfolio.RemoveAt(EditingPurchaseId);
                    vm.Portfolio.Insert(EditingPurchaseId, vm.NewPurchase);
                }

                /// Update the page and save the new portfolio
                UpdatePortfolio();
                LocalStorageHelper.SaveObject(PortfolioKey, vm.Portfolio);
            }
        }

        private async void DialogBtn_LostFocus(object sender, RoutedEventArgs e) {
            // If we change the crypto, set the current price to 0 so everything updates
            if (sender.GetType().Name == "ComboBox")
                vm.NewPurchase.Current = 0;

            // If we have the coin and the quantity, we can update some properties
            if (!string.IsNullOrEmpty(vm.NewPurchase.Crypto) && vm.NewPurchase.CryptoQty > 0)
                vm.NewPurchase = await PortfolioHelper.UpdatePurchase(vm.NewPurchase);
        }


        private void TimeRangeButtons_Tapped(object sender, TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;

            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
            UpdatePortfolio();
        }


        /// #######################################################################################
        /// Sorting
        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            else
                e.Column.SortDirection = DataGridSortDirection.Descending;

            switch (e.Column.Header) {
                case "Crypto":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Crypto ascending
                                                                                           select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Crypto descending
                                                                                           select item);
                    break;
                case "Invested":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.InvestedQty ascending
                                                                                           select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.InvestedQty descending
                                                                                           select item);
                    break;
                case "Worth":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Worth ascending
                                                                                           select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Worth descending
                                                                                           select item);
                    break;
                case "Currently":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Current ascending
                                                                                           select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Current descending
                                                                                           select item);
                    break;
            }
            foreach (var dgColumn in Portfolio_dg.Columns) {
                if (dgColumn.Header.ToString() != e.Column.Header.ToString())
                    dgColumn.SortDirection = null;
            }
        }

        /// #######################################################################################
        /// Grouping
        public class GroupInfoCollection<T> : ObservableCollection<T> {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator() {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

        private void dg_loadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e) {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            e.RowGroupHeader.PropertyValue = ((GroupInfoCollection<PurchaseModel>)group.Group).Key.ToString();
        }

        private void Grouping_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var opt = ((ContentControl)((Selector)sender).SelectedItem).Content.ToString();

            if (vm.Portfolio == null || Portfolio_dg == null)
                return;

            if (opt == "Dont group") {
                Portfolio_dg.ItemsSource = vm.Portfolio;
            }
            else {
                ObservableCollection<GroupInfoCollection<PurchaseModel>> purchases = new ObservableCollection<GroupInfoCollection<PurchaseModel>>();
                switch (opt) {
                    case "By profits":
                        var query = from item in vm.Portfolio
                                    group item by (item.Profit >= 0 ? "win" : "loss")
                                    into g select new { GroupName = g.Key, Items = g };
                        foreach (var g in query) {
                            GroupInfoCollection<PurchaseModel> info = new GroupInfoCollection<PurchaseModel>();
                            info.Key = (g.GroupName == "win") ? "Profits" : "Losses";
                            foreach (var item in g.Items) {
                                info.Add(item);
                            }
                            purchases.Add(info);
                        }
                        break;
                    case "By coin":
                        query = from item in vm.Portfolio
                                group item by item.Crypto
                                into g select new { GroupName = g.Key, Items = g };
                        foreach (var g in query) {
                            GroupInfoCollection<PurchaseModel> info = new GroupInfoCollection<PurchaseModel>();
                            info.Key = g.GroupName;
                            foreach (var item in g.Items) {
                                info.Add(item);
                            }
                            purchases.Add(info);
                        }
                        break;
                }

                CollectionViewSource groupedItems = new CollectionViewSource();
                groupedItems.IsSourceGrouped = true;
                groupedItems.Source = purchases;

                Portfolio_dg.ItemsSource = groupedItems.View;
            }
        }

        /// #######################################################################################
        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e) {
            AutoSuggestBox box = sender as AutoSuggestBox;
            box.ItemsSource = FilterCoins(box);
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
            => CoinAutoSuggestBox.Text = ((SuggestionCoin)args.SelectedItem).Symbol;

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            vm.NewPurchase.Crypto = ((SuggestionCoin)args.ChosenSuggestion)?.Symbol;
            vm.NewPurchase.CryptoName = ((SuggestionCoin)args.ChosenSuggestion)?.Name;
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                sender.ItemsSource = FilterCoins(sender);
        }

        private List<SuggestionCoin> FilterCoins(AutoSuggestBox box) {
            var filtered = App.coinList.Where(x =>
                x.symbol.Contains(box.Text, StringComparison.InvariantCultureIgnoreCase) ||
                x.name.Contains(box.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            List<SuggestionCoin> list = new List<SuggestionCoin>();
            foreach (CoinBasicInfo coin in filtered) {
                list.Add(new SuggestionCoin {
                    Icon = IconsHelper.GetIcon(coin.symbol),
                    Name = coin.name,
                    Symbol = coin.symbol
                });
            }
            return list;
        }
    }
}
