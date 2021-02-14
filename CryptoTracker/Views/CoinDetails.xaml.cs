using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using CryptoTracker.UserControls;
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
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        // Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;

        public CoinDetails() {
            this.InitializeComponent();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            /// Create the connected animation
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
            if (animation != null)
                animation.TryStart(PriceChart, new UIElement[] { BottomCards });

            /// Create the 30sec auto-refresh
            TimeSpan period = TimeSpan.FromSeconds(30);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    if (timeUnit == "minute" && this.Frame.SourcePageType.Name == "CoinDetails")
                        TimeRangeButtons_Tapped(null, null);
                });
            }, period);

            try {
                var type = (e.Parameter.GetType()).Name;
                if (type == nameof(HomeCard)) {
                    viewModel.Chart = ((HomeCard)e.Parameter).Chart;
                    viewModel.Coin = ((HomeCard)e.Parameter).Info;
                    var c = viewModel.Coin;
                    var coin = App.coinList.Find(x => x.symbol == c.Name);
                    viewModel.Coin.FullName = coin.name;
                    FavIcon.Content = viewModel.Coin.IsFav ? "\uEB52" : "\uEB51";
                    // TODO: update info and market info
                }
                else {
                    // Page title
                    var crypto = e.Parameter?.ToString().ToUpperInvariant() ?? "NULL";
                    viewModel.Coin.Name = crypto;
                    var coin = App.coinList.Find(x => x.symbol == crypto);
                    viewModel.Coin.FullName = coin.name;
                    FavIcon.Content = App.pinnedCoins.Contains(crypto) ? "\uEB52" : "\uEB51";

                    InitValuesFromZero(coin);
                }

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

        /// #########################################################################################
        private async void InitValuesFromZero(CoinBasicInfo coin) {
            viewModel.CoinInfo = await API_CoinGecko.GetCoin(coin.name);

            try {
                TimeRangeButtons_Tapped(null, null);

            } catch (Exception) {
                viewModel.Coin.IsLoading = false;
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
            var brush = viewModel.Chart.ChartStroke;
            if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
            viewModel.Chart.ChartStroke = brush;

            /// Get Historic and create List of ChartData for the chart (plus LinearAxis)
            var histo = await CryptoCompare.GetHistoricAsync(crypto, timeUnit, limit, aggregate);
            var chartData = new List<ChartPoint>();
            foreach (var h in histo) {
                chartData.Add(new ChartPoint() {
                    Color = brush.Color,
                    Date = h.DateTime,
                    Value = h.Average,
                    Volume = h.volumefrom
                });
            }
            viewModel.Chart.ChartData = chartData;
            var temp = App.AdjustLinearAxis(new ChartStyling(), timeSpan);
            viewModel.Chart.LabelFormat = temp.LabelFormat;
            viewModel.Chart.Minimum = temp.Minimum;
            viewModel.Chart.MajorStepUnit = temp.MajorStepUnit;
            viewModel.Chart.MajorStep = temp.MajorStep;

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            viewModel.Chart.PricesMinMax = MinMax;

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

        private void TimeRangeButtons_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;
            var t = App.TimeSpanParser(timeSpan);
            limit = t.limit;
            aggregate = t.aggregate;
            timeUnit = t.timeUnit;

            UpdatePage();
        }
    }
}
