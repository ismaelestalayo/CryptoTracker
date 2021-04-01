using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Background;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.UserControls;
using UWP.ViewModels;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace UWP.Views {
    public sealed partial class CoinDetails : Page {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";
        private double low = 0;
        private double high = 0;

        /// Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;

        public CoinDetails() {
            InitializeComponent();

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
                        vm.CoinInfo = await CoinGecko.GetCoin(coin.name);
                        break;
                    case nameof(CoinDetailsViewModel):
                        vm = (CoinDetailsViewModel)e.Parameter;
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
            vm.CoinInfo = await CoinGecko.GetCoin(coin.name);

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
            //await Get24Volume();
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
            low = MinMax.Min;
            high = MinMax.Max;
            vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max);

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
            vm.Coin.IsFav = !vm.Coin.IsFav;
            if (!App.pinnedCoins.Contains(crypto)) {
                App.pinnedCoins.Add(crypto);
                //Home.AddCoinHome(crypto);
                vm.InAppNotification($"{crypto} pinned to home.");
            }
            else {
                //Home.RemoveCoinHome(crypto);
                App.pinnedCoins.Remove(crypto);
                vm.InAppNotification($"{crypto} unpinned from home.");
            }
        }

        private async void PinCoin_click(object sender, RoutedEventArgs e) {
            bool success;
            if (vm.Coin.IsPin) {
                success = await LiveTileUpdater.RemoveSecondaryTileAction(vm.Coin.Name);
                /// reset it even if it fails
                vm.Coin.IsPin = false;
                if (success)
                    vm.InAppNotification($"Unpinned {vm.Coin.Name} from start screen");
            }
            else {
                var grid = await LiveTileGenerator.SecondaryTileGridOperation(vm.Coin.Name);

                try {
                    RenderTargetBitmap rtb = new RenderTargetBitmap();
                    BottomCards.Children.Add(grid);
                    grid.Opacity = 0;
                    await rtb.RenderAsync(grid);
                    BottomCards.Children.Remove(grid);
                    var pixelBuffer = await rtb.GetPixelsAsync();
                    var pixels = pixelBuffer.ToArray();
                    var displayInformation = DisplayInformation.GetForCurrentView();
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"tile-{vm.Coin.Name}.png",
                        CreationCollisionOption.ReplaceExisting);
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                        encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                             BitmapAlphaMode.Premultiplied,
                                             (uint)rtb.PixelWidth,
                                             (uint)rtb.PixelHeight,
                                             displayInformation.RawDpiX,
                                             displayInformation.RawDpiY,
                                             pixels);
                        await encoder.FlushAsync();
                    }
                }
                catch (Exception ex) {
                    var z = ex.Message;
                }

                success = await LiveTileUpdater.AddSecondaryTileAction(vm.Coin.Name);
                if (success) {
                    vm.Coin.IsPin = true;
                    vm.InAppNotification($"Pinned {vm.Coin.Name} to start screen");
                }
            }
        }

        private async void CompactOverlay_btn_click(object sender, RoutedEventArgs e) {
            var crypto = vm.Coin.Name;
            var view = ApplicationView.GetForCurrentView();

            var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            preferences.CustomSize = new Windows.Foundation.Size(350, 200);

            await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            Frame.Navigate(typeof(CoinCompact), vm, new SuppressNavigationTransitionInfo());
        }

        private void TimeRangeButtons_Tapped(object sender, TappedRoutedEventArgs e) {
            if (sender != null)
                timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;

            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
            vm.Chart.TimeSpan = timeSpan;

            UpdatePage();
        }
    }
}
