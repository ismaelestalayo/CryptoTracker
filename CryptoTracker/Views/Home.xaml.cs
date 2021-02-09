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
        private static int limit = 1500;
        private static string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();

            UpdateCoinList();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            InitHome();
            viewModel.CoinCards.CollectionChanged += HomeCoinList_CollectionChanged;
        }

        private void HomeCoinList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            EmptyPageWarning.Visibility = (((Collection<CoinCard>)sender).Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        /// #########################################################################################
        private async void UpdateCoinList() {
            /// First keep an updated list of coins
            await App.GetCoinList();
        }

        private async void InitHome() {
            /// See if there's any change
            var pinned = App.pinnedCoins.ToList();
            var current = viewModel.CoinCards.Select(x => x.Crypto).ToList();

            // TODO: dont clear cards that haven't changed
            if(!pinned.SequenceEqual(current)) {
                viewModel.CoinCards.Clear();
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
            } catch (Exception) {

            }

            viewModel.CoinCards.Add(new CoinCard {
                Crypto = crypto
            });

            /// Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        internal void RemoveCoinHome(string crypto) {
            if (App.pinnedCoins.Contains(crypto)) {
                var n = App.pinnedCoins.IndexOf(crypto);

                App.pinnedCoins.RemoveAt(n);
                viewModel.CoinCards.RemoveAt(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        /// #########################################################################################
        ///  Update all cards
        internal async Task UpdateAllCards() {
            foreach (var homeCard in viewModel.CoinCards)
                homeCard.IsLoading = true;
            
            for (int i = 0; i < viewModel.CoinCards.Count; i++)
                await UpdateCard(i);
            
        }

        private async Task UpdateCard(int i) {
			try {
                string crypto = App.pinnedCoins[i];

                /// Get price
                viewModel.CoinCards[i].Price = await CryptoCompare.GetPriceAsync(crypto);

                /// Colors
                var brush = viewModel.CoinCards[i].ChartStroke;
                if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                    brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
                viewModel.CoinCards[i].ChartStroke = brush;

                /// Get Historic and create List of ChartData for the chart
                var histo = await CryptoCompare.GetHistoricAsync(crypto, timeSpan, limit);
                var chartData = new List<ChartData>();
				foreach (var h in histo) {
					chartData.Add(new ChartData() {
						Date = h.DateTime,
						Value = h.Average,
                        Volume = h.volumefrom,
                        Color = brush.Color
                    });
				}
                viewModel.CoinCards[i].ChartData = chartData;

                /// Calculate min-max to adjust axis
                var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
                viewModel.CoinCards[i].PricesMinMax = MinMax;

                /// Calculate the price difference
                double oldestPrice = histo[0].Average;
				double newestPrice = histo[histo.Count - 1].Average;
				double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
                viewModel.CoinCards[i].Diff = diff;

                /// Sum total volume from historic
                double total1 = 0, total2 = 0;
                histo.ForEach(x => total1 += x.volumeto);
                histo.ForEach(x => total2 += x.volumefrom);
                viewModel.CoinCards[i].Volume24 = NumberHelper.AddUnitPrefix(total2) + App.currencySymbol;
                viewModel.CoinCards[i].Volume24to = NumberHelper.AddUnitPrefix(total1) + App.currencySymbol;                

                /// Show that loading is done
                viewModel.CoinCards[i].IsLoading = false;
            } catch (Exception) {

            }
            
        }

        /// #########################################################################################
        private async void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    timeSpan = "minute";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "minute";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "hour";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "hour";
                    limit = 744;
                    break;
                case "year":
                    timeSpan = "day";
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "day";
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

            var clickedItem = (CoinCard)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem.Crypto);
        }

        private void UnpinCoin(object sender, RoutedEventArgs e) {
            string crypto = ((CoinCard)((FrameworkElement)sender).DataContext).Crypto;

            RemoveCoinHome(crypto);
        }
        private void MoveCoinDown(object sender, RoutedEventArgs e) {
            string crypto = ((CoinCard)((FrameworkElement)sender).DataContext).Crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if(n < viewModel.CoinCards.Count - 1) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n + 1];
                App.pinnedCoins[n + 1] = tempName;

                var tempListItem = viewModel.CoinCards[n];
                viewModel.CoinCards[n] = viewModel.CoinCards[n + 1];
                viewModel.CoinCards[n + 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
        private void MoveCoinUp(object sender, RoutedEventArgs e) {
            string crypto = ((CoinCard)((FrameworkElement)sender).DataContext).Crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if (n != 0) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n - 1];
                App.pinnedCoins[n - 1] = tempName;

                var tempListItem = viewModel.CoinCards[n];
                viewModel.CoinCards[n] = viewModel.CoinCards[n - 1];
                viewModel.CoinCards[n - 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
    }
}

