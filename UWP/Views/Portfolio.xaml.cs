using CryptoTracker.Dialogs;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using UWP.Shared.Interfaces;
using UWP.Shared.Models;
using UWP.UserControls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace UWP.Views {

    public partial class Portfolio : Page, UpdatablePage {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        private string PortfolioKey = "Portfolio";

        private ObservableCollection<PurchaseModel> LocalPurchases;

        private int EditingPurchaseId { get; set; }

        private bool ShowingDetails = false;

        public Portfolio() {
            this.InitializeComponent();   
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            LocalPurchases = await RetrievePortfolio();
            vm.Portfolio = LocalPurchases;
            if (vm.PurchasesAreGrouped)
                vm.Portfolio = await PortfolioHelper.GroupPortfolio(LocalPurchases);
            await UpdatePage();
        }

        public async Task UpdatePage() {
            /// Empty diversification chart and reset the Total amounts
            PortfolioChartGrid.ColumnDefinitions.Clear();
            PortfolioChartGrid.Children.Clear();

            /// Update the purchase details first
            for (int i = 0; i < vm.Portfolio.Count; i++)
                await PortfolioHelper.UpdatePurchase(vm.Portfolio[i]);

            vm.TotalInvested = vm.Portfolio.Sum(x => x.InvestedQty);
            vm.TotalWorth = vm.Portfolio.Sum(x => x.Worth);

            /// Create the diversification grid
            var grouped = vm.Portfolio.GroupBy(x => x.Crypto);
            foreach (var purchases in grouped) {
                var crypto = purchases.Key.ToUpperInvariant();
                var worth = purchases.ToList().Sum(x => x.Worth);

                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(worth, GridUnitType.Star);
                PortfolioChartGrid.ColumnDefinitions.Add(col);

                var s = new StackPanel();
                s.BorderThickness = new Thickness(0);

                // Tooltip
                var val = Math.Round((worth / vm.TotalWorth) * 100, 1);
                ToolTipService.SetToolTip(s, $"{val}%");
                ToolTipService.SetPlacement(s, PlacementMode.Bottom);

                var t = new TextBlock() {
                    Text = crypto,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 7, 0, 7)
                };
                s.Children.Add(t);
                s.Background = ColorConstants.GetCoinBrush(crypto + "_color", 100);

                PortfolioChartGrid.Children.Add(s);
                Grid.SetColumn(s, PortfolioChartGrid.Children.Count - 1);
            }

            /// Finally, update the chart of the portfolio's worth
            await UpdatePortfolioChart();
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
            UpdatePage();
            LocalStorageHelper.SaveObject(PortfolioKey, vm.Portfolio);
        }

        private async void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            var item = ((MenuFlyoutItem)sender).DataContext as PurchaseModel;
            vm.Portfolio.Remove(item);

            if (vm.PurchasesAreGrouped) {
                var crypto = item.Crypto;
                var matches = LocalPurchases.Where(x => x.Crypto == crypto).ToList();
                foreach (var match in matches)
                    LocalPurchases.Remove(match);
            }
            else {
                LocalPurchases = vm.Portfolio;
            }

            /// Update the page and save the new portfolio
            await UpdatePage();
            await LocalStorageHelper.SaveObject(PortfolioKey, LocalPurchases);
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
        private async void AddPurchase_click(object sender, RoutedEventArgs e) {
            var dialog = new PortfolioEntryDialog() {
                NewPurchase = new PurchaseModel(),
                PrimaryButtonText = "Add",
                Title = "💵 New purchase"
            };
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary) {
                dialog.NewPurchase.CryptoName = App.coinListPaprika.FirstOrDefault(
                    x => x.symbol == dialog.NewPurchase.Crypto).name;
                vm.Portfolio.Add(dialog.NewPurchase);
                PortfolioHelper.AddPurchase(dialog.NewPurchase);
                
                // Update everything
                UpdatePage();
            }
        }

        private async void EditPurchase_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;
            var dialog = new PortfolioEntryDialog() {
                NewPurchase = purchase,
                PrimaryButtonText = "Save",
                Title = "💵 Edit purchase"
            };
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary) {
                EditingPurchaseId = vm.Portfolio.IndexOf(purchase);
                vm.Portfolio.RemoveAt(EditingPurchaseId);
                vm.Portfolio.Insert(EditingPurchaseId, dialog.NewPurchase);
                PortfolioHelper.SavePortfolio(vm.Portfolio);

                // Update everything
                UpdatePage();
            }
        }

        private void TimeRangeButtons_Tapped(object sender, TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;

            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
            UpdatePage();
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
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Crypto ascending
                                                                                           select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Crypto descending
                                                                                           select item);
                    break;
                case "Invested":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.InvestedQty ascending
                                                                                           select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.InvestedQty descending
                                                                                           select item);
                    break;
                case "Worth":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Worth ascending
                                                                                           select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Worth descending
                                                                                           select item);
                    break;
                case "Currently":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
                                                                                           orderby item.Current ascending
                                                                                           select item);
                    else
                        vm.Portfolio = new ObservableCollection<PurchaseModel>(from item in vm.Portfolio
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
        /// Manual grouping
        private async void GroupPurchases_Click(object sender, RoutedEventArgs e) {
            if (vm.PurchasesAreGrouped) {
                // group the same coins' purchases into one single entry
                vm.Portfolio = await PortfolioHelper.GroupPortfolio(LocalPurchases);
            }
            else {
                // reset the List to the local purchases
                vm.Portfolio = LocalPurchases;
                await UpdatePage();
            }
        }
    }
}
