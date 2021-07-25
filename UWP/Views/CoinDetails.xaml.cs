using CryptoTracker.Dialogs;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Background;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Helpers;
using UWP.Shared.Interfaces;
using UWP.UserControls;
using UWP.ViewModels;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace UWP.Views {
    public sealed partial class CoinDetails : Page, UpdatablePage {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        /// Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;

        public CoinDetails() {
            InitializeComponent();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            /// If list is empty
            if (App.coinListPaprika.Count == 0)
                await App.GetCoinList();

            /// Create the connected animation
            var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
            if (animation != null)
                animation.TryStart(PriceChart, new UIElement[] { BottomUniformGrid });

            /// Create the auto-refresh timer
            var autoRefresh = App._LocalSettings.Get<string>(UserSettings.AutoRefresh);
            TimeSpan period;
            if (autoRefresh != "None") {
                switch (autoRefresh) {
                    case "30 sec":
                        period = TimeSpan.FromSeconds(30);
                        break;
                    case "1 min":
                        period = TimeSpan.FromSeconds(60);
                        break;
                    case "2 min":
                        period = TimeSpan.FromSeconds(120);
                        break;
                }
                PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                        if (timeUnit == "minute")
                            TimeRangeButtons_Tapped(null, null);
                    });
                }, period);
            }

            try {
                var type = (e.Parameter.GetType()).Name;
                switch (type) {
                    case nameof(HomeCard):
                        vm.Chart = ((HomeCard)e.Parameter).Chart;
                        vm.Coin = ((HomeCard)e.Parameter).Info;
                        vm.Chart.TimeSpan = vm.Chart.TimeSpan;

                        var coin = App.coinListPaprika.Find(x => x.symbol == vm.Coin.Name);
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
                        var _coin = App.coinListPaprika.Find(x => x.symbol == crypto) ?? new CoinPaprikaCoin();
                        vm.Coin.FullName = _coin?.name ?? "NULL";

                        InitValuesFromZero(_coin.name);
                        break;
                }
            }
            catch (Exception ex){
                var message = $"There was an error loading that coin. Try again later.\n\n{ex.Message}";
                new MessageDialog(message).ShowAsync();
            }

            var portfolio = await PortfolioHelper.GetPortfolio(vm.Coin.Name);
            vm.Purchases = new ObservableCollection<PurchaseModel>(portfolio);
            for (int i = 0; i < vm.Purchases.Count; i++) {
                vm.Purchases[i] = await PortfolioHelper.UpdatePurchase(vm.Purchases[i]);
                vm.TotalOwned += vm.Purchases[i].CryptoQty;
                vm.TotalProfit += vm.Purchases[i].Profit;
                vm.TotalValue += vm.Purchases[i].Worth;
            }

            vm.Alerts = await AlertsHelper.GetCryptoAlerts(vm.Coin.Name);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            PeriodicTimer?.Cancel();
        }

        public async Task UpdatePage() {
            vm.Coin.IsLoading = true;

            await UpdateCoin();
            //CryptoCompare.GetExchanges(crypto);
        }

        /// #########################################################################################
        private async void InitValuesFromZero(string cryptoName) {
            vm.CoinInfo = await CoinGecko.GetCoin(cryptoName);

            try {
                TimeRangeButtons_Tapped(null, null);

            } catch (Exception) {
                vm.Coin.IsLoading = false;
            }
        }

        /// #########################################################################################
        /// #########################################################################################
        /// #########################################################################################
        private async Task UpdateCoin() {
            var crypto = vm.Coin.Name;
            vm.Coin.Price = await Ioc.Default.GetService<ICryptoCompare>().GetPrice_Extension(
                crypto, App.currency);

            /// Colors
            var brush = vm.Chart.ChartStroke;
            if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
            vm.Chart.ChartStroke = brush;

            /// Get Historic and create List of ChartData for the chart (plus LinearAxis)
            var histo = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric_(crypto, timeUnit, limit, aggregate);
            var chartData = new List<ChartPoint>();
            foreach (var h in histo) {
                chartData.Add(new ChartPoint() {
                    Date = h.DateTime,
                    Value = h.Average,
                    Volume = h.volumeto,
                    High = h.high,
                    Low = h.low,
                    Open = h.open,
                    Close = h.close
                });
            }
            vm.Chart.ChartData = chartData;
            var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
            vm.Chart.LabelFormat = temp.LabelFormat;
            vm.Chart.Minimum = temp.Minimum;
            vm.Chart.MajorStepUnit = temp.MajorStepUnit;
            vm.Chart.MajorStep = temp.MajorStep;
            vm.Chart.TickInterval = temp.TickInterval;

            vm.Coin.VolumeToTotal = histo.Sum(x => x.volumeto);

            /// Calculate min-max to adjust axis
            var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
            vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max);
            vm.Chart.VolumeMax = GraphHelper.GetMaxOfVolume(chartData);

            /// Calculate the price difference
            double oldestPrice = histo[0].Average;
            double newestPrice = histo[histo.Count - 1].Average;
            vm.Coin.Diff = newestPrice - oldestPrice;

            vm.Coin.IsLoading = false;
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
                    BottomUniformGrid.Children.Add(grid);
                    grid.Opacity = 0;
                    await rtb.RenderAsync(grid);
                    BottomUniformGrid.Children.Remove(grid);
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

        private void ShowCandles_Click(object sender, RoutedEventArgs e) {
            vm.ShowCandles = !vm.ShowCandles;
        }

        private async void NewPurchase_click(object sender, RoutedEventArgs e) {
            var dialog = new PortfolioEntryDialog() {
                NewPurchase = new PurchaseModel() { Crypto = vm.Coin.Name }
            };
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary) {
                vm.Purchases.Add(dialog.NewPurchase);
                PortfolioHelper.AddPurchase(dialog.NewPurchase);
            }
        }

        public string Format(string text, object arg) => string.Format(text, arg);

        private void NewAlert(object sender, RoutedEventArgs e) {
            vm.Alerts.Add(new Alert() {
                Crypto = vm.Coin.Name,
                Currency = App.currency,
                CurrencySymbol = App.currencySymbol,
                Enabled = true,
                Id = Guid.NewGuid().GetHashCode(),
                Mode = "above"
            });
        }

        private void Flyout_Closed(object sender, object e)
            => AlertsHelper.UpdateOneCryptoAlerts(vm.Coin.Name, vm.Alerts);
    }
}
