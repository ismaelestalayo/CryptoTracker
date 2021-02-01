using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Model;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
	public sealed partial class Home : Page {

        static ObservableCollection<HomeTile> homeCoinList { get; set; }
        private static int limit = 1500;
        private static string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            homeCoinList = new ObservableCollection<HomeTile>();
            PriceListView.ItemsSource = homeCoinList;
            VolumeListView.ItemsSource = homeCoinList;

            InitHome();

            homeCoinList.CollectionChanged += HomeCoinList_CollectionChanged;
        }

        private void HomeCoinList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            EmptyPageWarning.Visibility = (((Collection<HomeTile>)sender).Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void InitHome() {
            // First keep an updated list of coins
            await App.GetCoinList();

			// Then update home-coin-tiles
			try {
				foreach (var coin in App.pinnedCoins)
                    await AddCoinHome(coin);
                for (int i = 0; i < App.pinnedCoins.Count; i++)
                    await UpdateCard(i);
            } catch {

			}
            
        }

        // #########################################################################################
        // Add/remove coins from Home
        internal static async Task AddCoinHome(string crypto) {

            if (crypto == "MIOTA")
                crypto = "IOT";

            String iconPath = "/Assets/Icons/icon" + crypto + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch (Exception) {

            }

            homeCoinList.Add(new HomeTile {
                CryptoName = crypto,
                Crypto = crypto,
                IconSrc = iconPath
            });

            // Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        internal static void RemoveCoinHome(string crypto) {
            if (App.pinnedCoins.Contains(crypto)) {
                var n = App.pinnedCoins.IndexOf(crypto);

                App.pinnedCoins.RemoveAt(n);
                homeCoinList.RemoveAt(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        /// #########################################################################################
        ///  Update all cards
        internal async Task UpdateAllCards() {
            foreach (var homeTile in homeCoinList) {
                homeTile.Opacity = 0.33;
                homeTile.IsLoading = true;
            }
            
            for (int i = 0; i < homeCoinList.Count; i++)
                await UpdateCard(i);
            
        }

        private async Task UpdateCard(int i) {
            
            string crypto = App.pinnedCoins[i];

			try {
                /// COLOR
                SolidColorBrush colorBrush = (SolidColorBrush)Application.Current.Resources["Main_WhiteBlack"];
                if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                    colorBrush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];

                var color = colorBrush.Color;
                homeCoinList[i].ChartFill1 = Color.FromArgb(62, color.R, color.G, color.B);
                homeCoinList[i].ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
                homeCoinList[i].ChartStroke = colorBrush;

                /// DATA
                homeCoinList[i].Price = await CryptoCompare.GetPriceAsync(crypto);
                var histo = await CryptoCompare.GetHistoricAsync(crypto, timeSpan, limit);

                /// Create List of ChartData for the chart
                var chartData = new List<ChartData>();
				foreach (var h in histo) {
					chartData.Add(new ChartData() {
						Date = h.DateTime,
						Value = h.Average,
                        Volume = h.volumefrom,
                        Color = color
                    });
				}
                homeCoinList[i].ChartData = chartData;

                double oldestPrice = histo[0].Average;
				double newestPrice = histo[histo.Count - 1].Average;
				double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

                if (diff > 0) {
                    homeCoinList[i].PriceDiff = diff;
                    var brush = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                }
                else {
                    diff = Math.Abs(diff);
                    homeCoinList[i].PriceDiff = diff;
                    var brush = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                }

                /// Sum total volume from historic
                double total1 = 0, total2 = 0;
                histo.ForEach(x => total1 += x.volumeto);
                histo.ForEach(x => total2 += x.volumefrom);
                homeCoinList[i].Volume24 = App.ToKMB(total2) + App.coinSymbol;
                homeCoinList[i].Volume24to = App.ToKMB(total1) + App.coinSymbol;



                /// #########################################################################################
                /// PRICE CHART

                var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
                homeCoinList[i].PricesMinMax = MinMax;
                


                homeCoinList[i].IsLoading = false;
                homeCoinList[i].Opacity = 1;
            } catch (Exception e) {
                Analytics.TrackEvent("UNHANDLED2: " + e.Message);
            }
            
        }

        // #########################################################################################
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
            // Connected animation
            switch ( ((ListView)sender).Name ) {
                case "PriceListView":
                    PriceListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "PriceListView_Element");
                    break;

                case "VolumeListView":
                    VolumeListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "VolumeListView_Element");
                    break;
            }

            var clickedItem = (HomeTile)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem.Crypto);
        }

        private void UnpinCoin(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext).Crypto;

            RemoveCoinHome(crypto);
        }
        private void MoveCoinDown(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext).Crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if(n < homeCoinList.Count - 1) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n + 1];
                App.pinnedCoins[n + 1] = tempName;

                var tempListItem = homeCoinList[n];
                homeCoinList[n] = homeCoinList[n + 1];
                homeCoinList[n + 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
        private void MoveCoinUp(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext).Crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if (n != 0) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n - 1];
                App.pinnedCoins[n - 1] = tempName;

                var tempListItem = homeCoinList[n];
                homeCoinList[n] = homeCoinList[n - 1];
                homeCoinList[n - 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
    }
}

