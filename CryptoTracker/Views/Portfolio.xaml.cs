using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {

	public class SuggestionCoinList {
        public string Icon { get; set; }
        public string Name { get; set; }
    }

    public partial class Portfolio : Page {

        internal static bool ForceRefresh { get; set; }
        internal static ObservableCollection<PurchaseClass> PurchaseList { get; set; }
        internal ObservableCollection<PurchaseClass> NewPurchase { get; set; }
        internal static List<string> coinsArray = new List<string>();
        private int EditingPurchaseId { get; set; }

        private bool ShowingDetails = false;
        private string currTimerange = "month";

        private double _invested = 0;
        private double _worth = 0;

        public Portfolio() {
            this.InitializeComponent();

            PurchaseList = LocalStorageHelper.ReadObject<ObservableCollection<PurchaseClass>>("portfolio").Result;
            Portfolio_dg.ItemsSource = PurchaseList;

			PurchaseList.CollectionChanged += PurchaseList_CollectionChanged;
            PurchaseList_CollectionChanged(null, null);

            UpdatePortfolio();
        }

		private void PurchaseList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            PortfolioChartGrid.Visibility = (PurchaseList.Count == 0) ? Visibility.Collapsed : Visibility.Visible;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e) {
			RadioButton r = new RadioButton { Content = currTimerange };
			TimerangeButton_Click(r, null);

            if (coinsArray.Count == 0) {
                coinsArray = App.coinList.Select(x => x.symbol).ToList();
                coinsArray.Sort((x, y) => x.CompareTo(y));
            }
            

            if (ForceRefresh) {
				ForceRefresh = false;
				UpdatePortfolio();
				Portfolio_dg.ItemsSource = PurchaseList;
			}
		}


		// ###############################################################################################
		//  For sync all
		internal void UpdatePortfolio() {
            // empty diversification chart
            PortfolioChartGrid.ColumnDefinitions.Clear();
            PortfolioChartGrid.Children.Clear();

            _invested = 0;
            _worth = 0;

            foreach (PurchaseClass purchase in PurchaseList) {
                // this update the ObservableCollection itself
                UpdatePurchaseAsync(purchase);

                // create the diversification grid
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(purchase.Worth, GridUnitType.Star);
                PortfolioChartGrid.ColumnDefinitions.Add(col);

                var s = new StackPanel();
                s.BorderThickness = new Thickness(0);
                s.Margin = new Thickness(1, 0, 1, 0);
                var t = new TextBlock() {
                    Text = purchase.Crypto,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 7, 0, 7)
                };
                s.Children.Add(t);
                try { s.Background = (SolidColorBrush)App.Current.Resources[purchase.Crypto + "_colorT"]; }
                catch { s.Background = (SolidColorBrush)App.Current.Resources["Main_WhiteBlackT"]; }

                PortfolioChartGrid.Children.Add(s);
                Grid.SetColumn(s, PortfolioChartGrid.Children.Count - 1);

                _invested += purchase.InvestedQty;
                _worth += purchase.Worth;
            }

            RadioButton r = new RadioButton { Content = currTimerange };
            TimerangeButton_Click(r, null);

            total_invested.Text = _invested.ToString() + App.coinSymbol;
            total_worth.Text = _worth.ToString() + App.coinSymbol;
        }

        internal async Task<PurchaseClass> UpdatePurchaseAsync(PurchaseClass purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = await CryptoCompare.GetPriceAsync(crypto);

            var curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            // If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty > 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                purchase.arrow = earningz < 0 ? "▼" : "▲";
                purchase.BoughtAt = priceBought;
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(Math.Abs(earningz), 2);
                purchase.ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
            }
            
            return purchase;
        }

        // ###############################################################################################
        private void GoToCoinPortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseClass;
            this.Frame.Navigate(typeof(CoinDetails), item.Crypto);
        }

        private void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseClass;
            var items = Portfolio_dg.ItemsSource.Cast<PurchaseClass>().ToList();
            var index = items.IndexOf(item);
            PurchaseList.RemoveAt(index);
            UpdatePortfolio();
            //SavePortfolio();
            LocalStorageHelper.SaveObject(PurchaseList, "portfolio");
        }


        // ###############################################################################################
        internal static void importPortfolio(ObservableCollection<PurchaseClass>portfolio) {
            PurchaseList = new ObservableCollection<PurchaseClass>(portfolio);
            LocalStorageHelper.SaveObject(PurchaseList, "portfolio");
            ForceRefresh = true;
        }

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

        // ###############################################################################################
        // Add/Edit purchase dialog
        private void AddPurchase_click(object sender, RoutedEventArgs e) {
            NewPurchase = new ObservableCollection<PurchaseClass>() { new PurchaseClass() };
            TestRepeater.ItemsSource = NewPurchase;
            PurchaseDialog.Title = "💵 New purchase";
            PurchaseDialog.PrimaryButtonText = "Add";
            PurchaseDialog.ShowAsync();
        }

        private void EditPurchase_Click(object sender, RoutedEventArgs e) {
            var purchase = ((PurchaseClass)((FrameworkElement)sender).DataContext);
            EditingPurchaseId = PurchaseList.IndexOf(purchase);
            NewPurchase = new ObservableCollection<PurchaseClass>() { purchase };
            TestRepeater.ItemsSource = NewPurchase;
            PurchaseDialog.Title = "💵 Edit purchase";
            PurchaseDialog.PrimaryButtonText = "Save";
            PurchaseDialog.ShowAsync();
        }

        private void PurchaseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(NewPurchase[0].Crypto) || NewPurchase[0].CryptoQty <= 0 || NewPurchase[0].InvestedQty <= 0) {
                args.Cancel = true;
                new MessageDialog("Error.").ShowAsync();
            }
            else {
                // Get logo for the coin just in case the coin changed
                var crypto = NewPurchase[0].Crypto;
                string logoURL = "Assets/Icons/icon" + crypto + ".png";
                if (!File.Exists(logoURL))
                    NewPurchase[0].CryptoLogo = "https://chasing-coins.com/coin/logo/" + crypto;
                else
                    NewPurchase[0].CryptoLogo = "/" + logoURL;

                if (sender.PrimaryButtonText == "Add") {
                    PurchaseList.Add(NewPurchase[0]);
                }
                else if(sender.PrimaryButtonText == "Save") {
                    PurchaseList.RemoveAt(EditingPurchaseId);
                    PurchaseList.Insert(EditingPurchaseId, NewPurchase[0]);
                }
                // Update and save
                UpdatePortfolio();
                LocalStorageHelper.SaveObject(PurchaseList, "portfolio");
            }
        }

        private async void DialogBtn_LostFocus(object sender, RoutedEventArgs e) {
            // If we change the crypto, set the current price to 0 so everything updates
            if (sender.GetType().Name == "ComboBox")
                NewPurchase[0].Current = 0;

            // If we have the coin and the quantity, we can update some properties
            if (!string.IsNullOrEmpty(NewPurchase[0].Crypto) && NewPurchase[0].CryptoQty > 0)
                NewPurchase[0] = await UpdatePurchaseAsync(NewPurchase[0]);
        }

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            else
                e.Column.SortDirection = DataGridSortDirection.Descending;

            switch (e.Column.Header) {
                case "Crypto":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Crypto ascending
                                                                                        select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Crypto descending
                                                                                        select item);
                    break;
                case "Invested":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.InvestedQty ascending
                                                                                        select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.InvestedQty descending
                                                                                        select item);
                    break;
                case "Worth":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Worth ascending
                                                                                        select item);
                    else
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Worth descending
                                                                                        select item);
                    break;
                case "Currently":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Current ascending
                                                                                        select item);
                    else 
                        Portfolio_dg.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Current descending
                                                                                        select item);
                    break;
            }
            foreach (var dgColumn in Portfolio_dg.Columns) {
                if (dgColumn.Header.ToString() != e.Column.Header.ToString())
                    dgColumn.SortDirection = null;
            }
        }

        private async void TimerangeButton_Click(object sender, RoutedEventArgs e) {
            var nPurchases = PurchaseList.Count;
            if (nPurchases == 0)
                return;

            RadioButton btn = sender as RadioButton;
            currTimerange = btn.Content.ToString();

            var t = App.ParseTimeSpan(currTimerange);
            int limit = t.Item2;


            var k = new List<List<JSONhistoric>>(nPurchases);
            foreach (PurchaseClass purchase in PurchaseList) {
                var hist = await App.GetHistoricalPrices(purchase.Crypto, currTimerange);
                k.Add(hist);
            }

            var dates_arr = k[0].Select(kk => kk.DateTime).ToList();
            var arr = k.Select(kk => kk.Select(kkk => kkk.High)).ToArray();
            var prices = new List<List<double>>();
            for (int i = 0; i < arr.Length; i++) {
                prices.Add(arr[i].Select(a => a * PurchaseList[i].CryptoQty).ToList());
            }

            var prices_arr = new List<double>();
            var min_limit = prices.Select(x => x.Count).Min();
            for (int i = 0; i < min_limit; i++) {
                prices_arr.Add(prices.Select(p => p[i]).Sum());
            }

            List<ChartData> data = new List<ChartData>();
            for (int i = 0; i < min_limit; ++i) {
                data.Add(new ChartData {
                    Date = dates_arr[i],
                    Value = (float)prices_arr[i]
                });
            }
            var series = (SplineAreaSeries)PortfolioChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            var MinMax = GraphHelper.GetMinMaxOfArray(data.Select(d => d.Value).ToList());
            verticalAxis.Minimum = MinMax.min;
            verticalAxis.Maximum = MinMax.max;
            dateTimeAxis = App.AdjustAxis(dateTimeAxis, currTimerange);
        }

        public class GroupInfoCollection<T> : ObservableCollection<T> {
            public object Key { get; set; }

            public new IEnumerator<T> GetEnumerator() {
                return (IEnumerator<T>)base.GetEnumerator();
            }
        }

		private void dg_loadingRowGroup(object sender, DataGridRowGroupHeaderEventArgs e) {
            ICollectionViewGroup group = e.RowGroupHeader.CollectionViewGroup;
            e.RowGroupHeader.PropertyValue = ((GroupInfoCollection<PurchaseClass>)group.Group).Key.ToString();
        }

		private void Grouping_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var opt = ((ContentControl)((Selector)sender).SelectedItem).Content.ToString();

            if (PurchaseList == null || Portfolio_dg == null)
                return;

            if (opt == "Dont group") {
                Portfolio_dg.ItemsSource = PurchaseList;
            }
            else {
                ObservableCollection<GroupInfoCollection<PurchaseClass>> purchases = new ObservableCollection<GroupInfoCollection<PurchaseClass>>();
                switch (opt) {
                    case "By profits":
                        var query = from item in PurchaseList group item by item.arrow into g select new { GroupName = g.Key, Items = g };
                        foreach (var g in query) {
                            GroupInfoCollection<PurchaseClass> info = new GroupInfoCollection<PurchaseClass>();
                            info.Key = (g.GroupName == "▲") ? "Profits" : "Losses";
                            foreach (var item in g.Items) {
                                info.Add(item);
                            }
                            purchases.Add(info);
                        }
                        break;
                    case "By coin":
                        query = from item in PurchaseList group item by item.Crypto into g select new { GroupName = g.Key, Items = g };
                        foreach (var g in query) {
                            GroupInfoCollection<PurchaseClass> info = new GroupInfoCollection<PurchaseClass>();
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
	}
}
