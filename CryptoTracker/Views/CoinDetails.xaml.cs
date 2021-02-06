using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {
	public sealed partial class CoinDetails : Page {
        private static int limit = 168;
        private static string timeSpan = "hour";
        private static ThreadPoolTimer PeriodicTimer;

        private Color ChartTrackBall = Color.FromArgb(255, 180, 50, 180);
        private SolidColorBrush ChartTrackBallBrush = new SolidColorBrush(Color.FromArgb(255, 180, 50, 180));

        public CoinDetails() {
            this.InitializeComponent();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            try {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
                if (animation != null)
                    animation.TryStart(PriceChart, new UIElement[]{ BottomCards } );
                
                
                // Page title
                var crypto = e.Parameter?.ToString().ToUpperInvariant() ?? "NULL";
                viewModel.CoinCard.Crypto = crypto;

                var coin = App.coinList.Find(x => x.symbol == crypto);
                viewModel.CoinCard.CryptoFullName = coin.name;
                viewModel.CoinCard.CryptoSymbol = coin.symbol;

                FavIcon.Content = App.pinnedCoins.Contains(crypto) ? "\uEB52" : "\uEB51";

                InitValues(coin);
            }
            catch (Exception ex){
                var message = "There was an error loading that coin. Try again later.";
                new MessageDialog(message).ShowAsync();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            if (PeriodicTimer != null)
                PeriodicTimer.Cancel();
        }

        private async void InitValues(CoinBasicInfo coin) {

            viewModel.CoinInfo = await API_CoinGecko.GetCoin(coin.name);

            TimeSpan period = TimeSpan.FromSeconds(30);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour" && this.Frame.SourcePageType.Name == "CoinDetails")
                        TimerangeButton_Click(r, null);
                });
            }, period);

            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                TimerangeButton_Click(r, null);

            } catch (Exception) {
                viewModel.CoinCard.IsLoading = false;
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        /// #########################################################################################
        /// #########################################################################################
        /// #########################################################################################
        internal async void UpdatePage() {
            viewModel.CoinCard.IsLoading = true;

            await UpdateCoin();
            await Get24Volume();
            //CryptoCompare.GetExchanges(crypto);
        }

        /// #########################################################################################
        private async Task UpdateCoin() {
            var crypto = viewModel.CoinCard.Crypto;
            viewModel.CoinCard.Price = await CryptoCompare.GetPriceAsync(crypto);

            /// Get Historic and create List of ChartData for the chart
            var histo = await CryptoCompare.GetHistoricAsync(crypto, timeSpan, limit);
            var chartData = new List<ChartData>();
            foreach (var h in histo) {
                chartData.Add(new ChartData() {
                    Color = viewModel.CoinCard.ChartStroke.Color,
                    Date = h.DateTime,
                    Value = h.Average,
                    Volume = h.volumefrom
                });
            }
            viewModel.CoinCard.ChartData = chartData;

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            viewModel.CoinCard.PricesMinMax = MinMax;

            /// Calculate the price difference
            double oldestPrice = histo[0].Average;
            double newestPrice = histo[histo.Count - 1].Average;
            double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
            viewModel.CoinCard.Diff = diff;

            viewModel.CoinCard.IsLoading = false;
        }

        
        async private Task Get24Volume() {
            var crypto = viewModel.CoinCard.Crypto;
            await App.GetHisto(crypto, "hour", 24);

            List<ChartData> data = new List<ChartData>();
            for (int i = 0; i < 24; i++) {
                data.Add(new ChartData() {
                    Date   = App.historic[i].DateTime,
                    Volume = App.historic[i].Volumefrom
                });
            }
            this.volumeChart.DataContext = data;
        }

        // #########################################################################################
        private void TimerangeButton_Click(object sender, RoutedEventArgs e) {
            viewModel.CoinCard.IsLoading = true;

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
            UpdatePage();
        }

        private void PinCoin_btn(object sender, RoutedEventArgs e) {
            var crypto = viewModel.CoinCard.Crypto;
            if (!App.pinnedCoins.Contains(crypto)) {
                App.pinnedCoins.Add(crypto);
                //Home.AddCoinHome(crypto);
                FavIcon.Content = "\uEB52";
                inAppNotification.Show(crypto + " pinned to home.", 2000);
            }
            else {
                //Home.RemoveCoinHome(crypto);
                App.pinnedCoins.Remove(crypto);
                FavIcon.Content = "\uEB51";
                inAppNotification.Show(crypto + " unpinned from home.", 2000);
            }
        }

		private async void CompactOverlay_btn_click(object sender, RoutedEventArgs e) {
            var crypto = viewModel.CoinCard.Crypto;
            var view = ApplicationView.GetForCurrentView();

			var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
			preferences.CustomSize = new Windows.Foundation.Size(350, 250);

			await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            Frame.Navigate(typeof(CoinCompact), crypto, new SuppressNavigationTransitionInfo());
        }
	}
}
