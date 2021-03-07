using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using CryptoTracker.UserControls;
using CryptoTracker.ViewModels;
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

        /// Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;

        public CoinDetails() {
            this.InitializeComponent();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            /// If list is empty
            if (App.coinList.Count == 0)
                await App.GetCoinList();

            /// Create the connected animation
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
            if (animation != null)
                animation.TryStart(PriceChart, new UIElement[] { BottomCards });

            /// Create the auto-refresh timer
            TimeSpan period = TimeSpan.FromSeconds(30);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    if (timeUnit == "minute")
                        TimeRangeButtons_Tapped(null, null);
                });
            }, period);

            try {
                var type = (e.Parameter.GetType()).Name;
                switch (type) {
                    case nameof(HomeCard):
                        vm.Chart = ((HomeCard)e.Parameter).Chart;
                        vm.Coin = ((HomeCard)e.Parameter).Info;
                        vm.Chart.TimeSpan = vm.Chart.TimeSpan;

                        var coin = App.coinList.Find(x => x.symbol == vm.Coin.Name);
                        vm.Coin.FullName = coin.name;
                        Fav_btn.Content = vm.Coin.IsFav ? "\uEB52" : "\uEB51";
                        vm.CoinInfo = await API_CoinGecko.GetCoin(coin.name);
                        break;
                    case nameof(CoinCompactViewModel):
                        vm.Chart = ((CoinCompactViewModel)e.Parameter).Chart;
                        vm.Coin = ((CoinCompactViewModel)e.Parameter).Info;
                        timeSpan = vm.Chart.TimeSpan;
                        (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
                        await UpdateCoin();
                        break;
                    default:
                    case "string":
                        var crypto = e.Parameter?.ToString().ToUpperInvariant() ?? "NULL";
                        vm.Coin.Name = crypto;
                        var _coin = App.coinList.Find(x => x.symbol == crypto) ?? new CoinBasicInfo();
                        vm.Coin.FullName = _coin?.name ?? "NULL";
                        Fav_btn.Content = App.pinnedCoins.Contains(crypto) ? "\uEB52" : "\uEB51";

                        InitValuesFromZero(_coin);
                        break;
                }

            }
            catch (Exception ex){
                var message = $"There was an error loading that coin. Try again later.\n\n{ex.Message}";
                new MessageDialog(message).ShowAsync();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            PeriodicTimer?.Cancel();
        }

        /// #########################################################################################
        private async void InitValuesFromZero(CoinBasicInfo coin) {
            vm.CoinInfo = await API_CoinGecko.GetCoin(coin.name);

            try {
                TimeRangeButtons_Tapped(null, null);

            } catch (Exception) {
                vm.Coin.IsLoading = false;
            }
        }

        /// #########################################################################################
        /// #########################################################################################
        /// #########################################################################################
        internal async void UpdatePage() {
            vm.Coin.IsLoading = true;
            
            await UpdateCoin();
            await Get24Volume();
            //CryptoCompare.GetExchanges(crypto);
        }

        /// #########################################################################################
        private async Task UpdateCoin() {
            var crypto = vm.Coin.Name;
            vm.Coin.Price = await CryptoCompare.GetPriceAsync(crypto);

            /// Colors
            var brush = vm.Chart.ChartStroke;
            if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
            vm.Chart.ChartStroke = brush;

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
            vm.Chart.ChartData = chartData;
            var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
            vm.Chart.LabelFormat = temp.LabelFormat;
            vm.Chart.Minimum = temp.Minimum;
            vm.Chart.MajorStepUnit = temp.MajorStepUnit;
            vm.Chart.MajorStep = temp.MajorStep;

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            vm.Chart.PricesMinMax = MinMax;

            /// Calculate the price difference
            double oldestPrice = histo[0].Average;
            double newestPrice = histo[histo.Count - 1].Average;
            double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
            vm.Coin.Diff = diff;

            vm.Coin.IsLoading = false;
        }

        
        async private Task Get24Volume() {
            var crypto = vm.Coin.Name;
            // TODO: add volume chart
        }

        // #########################################################################################
        private void FavCoin_click(object sender, RoutedEventArgs e) {
            var crypto = vm.Coin.Name;
            if (!App.pinnedCoins.Contains(crypto)) {
                App.pinnedCoins.Add(crypto);
                //Home.AddCoinHome(crypto);
                Fav_btn.Content = "\uEB52";
                vm.InAppNotification($"{crypto} pinned to home.");
            }
            else {
                //Home.RemoveCoinHome(crypto);
                App.pinnedCoins.Remove(crypto);
                Fav_btn.Content = "\uEB51";
                vm.InAppNotification($"{crypto} unpinned from home.");
            }
        }

        private async void CompactOverlay_btn_click(object sender, RoutedEventArgs e) {
            var crypto = vm.Coin.Name;
            var view = ApplicationView.GetForCurrentView();

            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Windows.Foundation.Size(350, 250);

            await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            Frame.Navigate(typeof(CoinCompact), vm, new SuppressNavigationTransitionInfo());
        }

        private void TimeRangeButtons_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;
            
            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
            vm.Chart.TimeSpan = timeSpan;

            UpdatePage();
        }

        private async void PinCoin_click(object sender, RoutedEventArgs e) {
            var kk = new ChartArea() { ChartModel = vm.Chart };
            kk.ChartModel.ChartStroke = (SolidColorBrush)Application.Current.Resources["Main_WhiteBlack"];
            kk.Opacity = 0;
            MainGrid.Children.Add(kk);
            await LiveTileHelper.AddSecondaryTile(vm.Coin.Name, kk);
            MainGrid.Children.Remove(kk);
            //PinCoin_togglebtn.Content = check ? new FontIcon() { Glyph = "&#xE840;" } : new FontIcon() { Glyph = "&#xE196;" };
        }
    }
}
