using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
	public sealed partial class Home : Page {
        /// <summary>
        /// Local variables
        /// </summary>
        private static int limit = 168;
        private static string timeSpan = "week";
        private static string timeUnit = "hour";

        public Home() {
            this.InitializeComponent();
            UpdateCoinList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            InitHome();
            viewModel.PriceCards.CollectionChanged += HomeCoinList_CollectionChanged;
        }

        private void HomeCoinList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            EmptyPageWarning.Visibility = (((Collection<HomeCard>)sender).Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// #########################################################################################
        private async void UpdateCoinList() {
            /// First keep an updated list of coins
            await App.GetCoinList();
        }

        private async void InitHome() {
            /// See if there's any change
            var pinned = App.pinnedCoins.ToList();
            var current = viewModel.PriceCards.Select(x => x.Info.Name).ToList();

            // TODO: dont clear cards that haven't changed
            if(!pinned.SequenceEqual(current)) {
                viewModel.PriceCards.Clear();
                viewModel.VolumeCards.Clear();
				foreach (var coin in App.pinnedCoins)
                    await AddCoinHome(coin);
            }

            for (int i = 0; i < App.pinnedCoins.Count; i++)
                await UpdateCard(i);
        }

        /// #########################################################################################
        /// Add/remove coins from Home
        // TODO: make static so Top100 can add/remove coins
        internal async Task AddCoinHome(string crypto) {

            if (crypto == "MIOTA")
                crypto = "IOT";

            String iconPath = "/Assets/Icons/icon" + crypto + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch (Exception) { }

            var h = new HomeCard() { Info = new Coin() { Name = crypto } };
            viewModel.PriceCards.Add(h);
            viewModel.VolumeCards.Add(h);

            /// Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        internal void RemoveCoinHome(string crypto) {
            if (App.pinnedCoins.Contains(crypto)) {
                var n = App.pinnedCoins.IndexOf(crypto);

                App.pinnedCoins.RemoveAt(n);
                viewModel.PriceCards.RemoveAt(n);
                viewModel.VolumeCards.RemoveAt(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        /// #########################################################################################
        ///  Update all cards
        internal async Task UpdateAllCards() {
            foreach (var homeCard in viewModel.PriceCards)
                homeCard.Info.IsLoading = true;
            
            for (int i = 0; i < viewModel.PriceCards.Count; i++)
                await UpdateCard(i);
            
        }

        private async Task UpdateCard(int i) {
			try {
                string crypto = App.pinnedCoins[i];

                /// Get price
                viewModel.PriceCards[i].Info.Price = await CryptoCompare.GetPriceAsync(crypto);

                /// Colors
                var brush = viewModel.PriceCards[i].Chart.ChartStroke;
                if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                    brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
                viewModel.PriceCards[i].Chart.ChartStroke = brush;

                /// Get Historic and create List of ChartData for the chart
                var histo = await CryptoCompare.GetHistoricAsync(crypto, timeUnit, limit);
                var chartData = new List<ChartPoint>();
				foreach (var h in histo) {
					chartData.Add(new ChartPoint() {
						Date = h.DateTime,
						Value = h.Average,
                        Volume = h.volumefrom
                    });
				}
                viewModel.PriceCards[i].Chart.ChartData = chartData;
                viewModel.VolumeCards[i].Chart.ChartData = chartData;
                var temp = App.AdjustLinearAxis(new ChartStyling(), timeSpan);
                viewModel.PriceCards[i].Chart.LabelFormat = temp.LabelFormat;
                viewModel.PriceCards[i].Chart.Minimum = temp.Minimum;
                viewModel.PriceCards[i].Chart.MajorStepUnit = temp.MajorStepUnit;
                viewModel.PriceCards[i].Chart.MajorStep = temp.MajorStep;

                /// Calculate min-max to adjust axis
                var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
                viewModel.PriceCards[i].Chart.PricesMinMax = MinMax;

                /// Calculate the price difference
                double oldestPrice = histo[0].Average;
				double newestPrice = histo[histo.Count - 1].Average;
				double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
                viewModel.PriceCards[i].Info.Diff = diff;

                /// Sum total volume from historic
                double total = 0;
                histo.ForEach(x => total += x.volumeto);
				viewModel.VolumeCards[i].Info.Volume = NumberHelper.AddUnitPrefix(total) + App.currencySymbol;

				/// Show that loading is done
				viewModel.PriceCards[i].Info.IsLoading = false;
            } catch (Exception) {

            }
            
        }

        /// #########################################################################################
        private async void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;
            timeSpan = btn.Content.ToString();
            switch (timeSpan) {
                case "hour":
                    timeUnit = "minute";
                    limit = 60;
                    break;

                case "day":
                    timeUnit = "minute";
                    limit = 1500;
                    break;

                case "week":
                    timeUnit = "hour";
                    limit = 168;
                    break;

                case "month":
                    timeUnit = "hour";
                    limit = 744;
                    break;
                case "year":
                    timeUnit = "day";
                    limit = 365;
                    break;

                case "all":
                    timeUnit = "day";
                    limit = 0;
                    break;

            }

            await UpdateAllCards();
        }

        private void homeListView_Click(object sender, ItemClickEventArgs e) {
            /// Connected animation
            switch ( ((ListView)sender).Name ) {
                case "PriceListView":
                    PriceListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "PriceListView_Element");
                    break;

                case "VolumeListView":
                    VolumeListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "VolumeListView_Element");
                    break;
            }

            var card = ((HomeCard)e.ClickedItem);
            this.Frame.Navigate(typeof(CoinDetails), card);
        }

        private void UnpinCoin(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;

            RemoveCoinHome(crypto);
        }
        private void MoveCoinDown(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;
            int n = App.pinnedCoins.IndexOf(crypto);

            if(n < viewModel.PriceCards.Count - 1) {
                var tmpName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n + 1];
                App.pinnedCoins[n + 1] = tmpName;

                /// Save in a temporal variable the first card, and swap
                var tmpCard = viewModel.PriceCards[n];
                viewModel.PriceCards[n] = viewModel.PriceCards[n + 1];
                viewModel.PriceCards[n + 1] = tmpCard;

                tmpCard = viewModel.VolumeCards[n];
                viewModel.VolumeCards[n] = viewModel.VolumeCards[n + 1];
                viewModel.VolumeCards[n + 1] = tmpCard;

                //PriceListView.UpdateLayout();
                //VolumeListView.UpdateLayout();
                //UpdateCard(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
        private void MoveCoinUp(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;
            int n = App.pinnedCoins.IndexOf(crypto);

            if (n != 0) {
                var tmpName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n - 1];
                App.pinnedCoins[n - 1] = tmpName;

                /// Save in a temporal variable the first card, and swap
                var tmpCard = viewModel.PriceCards[n];
                viewModel.PriceCards[n] = viewModel.PriceCards[n - 1];
                viewModel.PriceCards[n + 1] = tmpCard;

                tmpCard = viewModel.VolumeCards[n];
                viewModel.VolumeCards[n] = viewModel.VolumeCards[n - 1];
                viewModel.VolumeCards[n - 1] = tmpCard;

                //PriceListView.UpdateLayout();
                //VolumeListView.UpdateLayout();
                //UpdateCard(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
    }
}

