using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using CryptoTracker.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
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
        private static string timeSpan = "week";
        private static string timeUnit = "hour";
        private static ThreadPoolTimer PeriodicTimer;

        public CoinDetails() {
            this.InitializeComponent();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            try {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
                if (animation != null)
                    animation.TryStart(PriceChart, new UIElement[] { BottomCards });

                // Page title
                var crypto = e.Parameter?.ToString().ToUpperInvariant() ?? "NULL";
                viewModel.Coin.Name = crypto;

                var coin = App.coinList.Find(x => x.symbol == crypto);
                viewModel.Coin.FullName = coin.name;

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
                    RadioButton r = new RadioButton { Content = timeUnit };
                    if (timeSpan == "hour" && this.Frame.SourcePageType.Name == "CoinDetails")
                        TimerangeButton_Click(r, null);
                });
            }, period);

            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                TimerangeButton_Click(r, null);

            } catch (Exception) {
                viewModel.Coin.IsLoading = false;
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        /// #########################################################################################
        /// #########################################################################################
        /// #########################################################################################
        internal async void UpdatePage() {
            viewModel.Coin.IsLoading = true;
            
            await UpdateCoin();
            await Get24Volume();
            //CryptoCompare.GetExchanges(crypto);
        }

        /// #########################################################################################
        private async Task UpdateCoin() {
            var crypto = viewModel.Coin.Name;
            viewModel.Coin.Price = await CryptoCompare.GetPriceAsync(crypto);

            /// Colors
            var brush = viewModel.ChartModel.ChartStroke;
            if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
            viewModel.ChartModel.ChartStroke = brush;

            /// Get Historic and create List of ChartData for the chart (plus LinearAxis)
            var histo = await CryptoCompare.GetHistoricAsync(crypto, timeUnit, limit);
            var chartData = new List<ChartPoint>();
            foreach (var h in histo) {
                chartData.Add(new ChartPoint() {
                    Color = brush.Color,
                    Date = h.DateTime,
                    Value = h.Average,
                    Volume = h.volumefrom
                });
            }
            viewModel.ChartModel.ChartData = chartData;
            var temp = App.AdjustLinearAxis(new ChartStyling(), timeSpan);
            viewModel.ChartModel.LabelFormat = temp.LabelFormat;
            viewModel.ChartModel.Minimum = temp.Minimum;
            viewModel.ChartModel.MajorStepUnit = temp.MajorStepUnit;
            viewModel.ChartModel.MajorStep = temp.MajorStep;

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            viewModel.ChartModel.PricesMinMax = MinMax;

            /// Calculate the price difference
            double oldestPrice = histo[0].Average;
            double newestPrice = histo[histo.Count - 1].Average;
            double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
            viewModel.Coin.Diff = diff;

            viewModel.Coin.IsLoading = false;
        }

        
        async private Task Get24Volume() {
            var crypto = viewModel.Coin.Name;
            // TODO: add volume chart
        }

        // #########################################################################################
        private void TimerangeButton_Click(object sender, RoutedEventArgs e) {
            viewModel.Coin.IsLoading = true;

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
            UpdatePage();
        }

        private void PinCoin_btn(object sender, RoutedEventArgs e) {
            var crypto = viewModel.Coin.Name;
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
            var crypto = viewModel.Coin.Name;
            var view = ApplicationView.GetForCurrentView();

            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Windows.Foundation.Size(350, 250);

            await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            Frame.Navigate(typeof(CoinCompact), crypto, new SuppressNavigationTransitionInfo());
        }
    }
}
