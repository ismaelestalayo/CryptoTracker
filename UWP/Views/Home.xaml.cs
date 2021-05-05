using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Background;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Views {
    public sealed partial class Home : Page {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        /// Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;


        public Home() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            /// First keep an updated list of coins
            await App.GetCoinList();

            InitHome();

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
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            PeriodicTimer?.Cancel();
        }

        /// #########################################################################################
        private async void InitHome() {
            /// See if there's any change
            var pinned = App.pinnedCoins.ToList();
            var current = vm.PriceCards.Select(x => x.Info.Name).ToList();

            // TODO: dont clear cards that haven't changed
            if(!pinned.SequenceEqual(current)) {
                vm.PriceCards.Clear();
                vm.VolumeCards.Clear();
                foreach (var coin in App.pinnedCoins)
                    AddCoinHome(coin);
            }

            var tiles = await SecondaryTile.FindAllAsync();
            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                await UpdateCard(i);
                vm.PriceCards[i].Info.IsPin = tiles.Any(tile => tile.TileId == App.pinnedCoins[i]);
                vm.VolumeCards[i].Info.IsPin = tiles.Any(tile => tile.TileId == App.pinnedCoins[i]);
            }

        }

        /// #########################################################################################
        /// Add/remove coins from Home
        internal void AddCoinHome(string crypto) {
            var h = new HomeCard() { Info = new Coin() { Name = crypto } };
            vm.PriceCards.Add(h);
            vm.VolumeCards.Add(h);

            /// Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        internal void RemoveCoinHome(string crypto) {
            if (App.pinnedCoins.Contains(crypto)) {
                var n = App.pinnedCoins.IndexOf(crypto);

                App.pinnedCoins.RemoveAt(n);
                vm.PriceCards.RemoveAt(n);
                vm.VolumeCards.RemoveAt(n);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        /// #########################################################################################
        ///  Update all cards
        internal async Task UpdateAllCards() {
            foreach (var homeCard in vm.PriceCards)
                homeCard.Info.IsLoading = true;
            
            for (int i = 0; i < vm.PriceCards.Count; i++)
                await UpdateCard(i);
        }

        private async Task UpdateCard(int i) {
            try {
                string crypto = App.pinnedCoins[i];
                vm.PriceCards[i].Info.CurrencySym = App.currencySymbol;

                /// Get price
                vm.PriceCards[i].Info.Price = await Ioc.Default.GetService<ICryptoCompare>().GetPrice_Extension(
                    crypto, App.currency);

                /// Save the current timeSpan for navigating to another page
                vm.PriceCards[i].Chart.TimeSpan = timeSpan;

                /// Colors
                var brush = vm.PriceCards[i].Chart.ChartStroke;
                if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                    brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
                vm.PriceCards[i].Chart.ChartStroke = brush;

                /// Get Historic and create List of ChartData for the chart
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
                vm.PriceCards[i].Chart.ChartData = chartData;
                vm.VolumeCards[i].Chart.ChartData = chartData;
                var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
                vm.PriceCards[i].Chart.LabelFormat = temp.LabelFormat;
                vm.PriceCards[i].Chart.Minimum = temp.Minimum;
                vm.PriceCards[i].Chart.MajorStepUnit = temp.MajorStepUnit;
                vm.PriceCards[i].Chart.MajorStep = temp.MajorStep;
                vm.PriceCards[i].Chart.TickInterval = temp.TickInterval;

                /// Calculate min-max to adjust axis
                var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
                vm.PriceCards[i].Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max);
                vm.PriceCards[i].Chart.VolumeMax = GraphHelper.GetMaxOfVolume(chartData);

                /// Calculate the price difference
                double oldestPrice = histo[0].Average;
                double newestPrice = histo[histo.Count - 1].Average;
                vm.PriceCards[i].Info.Diff = newestPrice - oldestPrice;

                /// Sum total volume from historic
                vm.VolumeCards[i].Info.VolumeToTotal = histo.Sum(h => h.volumeto);
                vm.VolumeCards[i].Info.VolumeFromTotal = histo.Sum(h => h.volumefrom);
                double total = histo.Sum(h => h.volumeto);

                /// Show that loading is done
                vm.PriceCards[i].Info.IsLoading = false;
            } catch (Exception) {  }
            
        }

        /// #######################################################################################
        /// (Left/Right) Click handlers
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

        private void UnfavCoin(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;
            RemoveCoinHome(crypto);
        }
        private async void PinUnpinCoin(object sender, RoutedEventArgs e) {
            var card = (HomeCard)((FrameworkElement)sender).DataContext;
            string crypto = card.Info.Name;

            var priceCard = vm.PriceCards.FirstOrDefault(c => c.Info.Name == crypto);
            if (priceCard == null)
                return;

            int i = vm.PriceCards.IndexOf(priceCard);
            bool success = false;
            if (card.Info.IsPin) {
                success = await LiveTileUpdater.RemoveSecondaryTileAction(crypto);
                vm.PriceCards[i].Info.IsPin = false;
                vm.InAppNotification($"Unpinned {crypto} from start screen.");
            }
            else {
                var grid = await LiveTileGenerator.SecondaryTileGridOperation(crypto);

                try {
                    RenderTargetBitmap rtb = new RenderTargetBitmap();
                    MainGrid.Children.Add(grid);
                    grid.Opacity = 0;
                    await rtb.RenderAsync(grid);
                    MainGrid.Children.Remove(grid);
                    var pixelBuffer = await rtb.GetPixelsAsync();
                    var pixels = pixelBuffer.ToArray();
                    var displayInformation = DisplayInformation.GetForCurrentView();
                    var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"tile-{crypto}.png",
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


                success = await LiveTileUpdater.AddSecondaryTileAction(crypto);
                if (success) {
                    vm.PriceCards[i].Info.IsPin = true;
                    vm.InAppNotification($"Pinned {crypto} to start screen.");
                }
            }
        }
        private void MoveCoinDown(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;
            int n = App.pinnedCoins.IndexOf(crypto);

            if(n < vm.PriceCards.Count - 1) {
                /// Swap downwards the N card
                (App.pinnedCoins[n + 1], App.pinnedCoins[n]) = (App.pinnedCoins[n], App.pinnedCoins[n + 1]);
                (vm.PriceCards[n + 1], vm.PriceCards[n]) = (vm.PriceCards[n], vm.PriceCards[n + 1]);
                (vm.VolumeCards[n + 1], vm.VolumeCards[n]) = (vm.VolumeCards[n], vm.VolumeCards[n + 1]);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
        private void MoveCoinUp(object sender, RoutedEventArgs e) {
            string crypto = ((HomeCard)((FrameworkElement)sender).DataContext).Info.Name;
            int n = App.pinnedCoins.IndexOf(crypto);

            if (n != 0) {
                /// Swap upwards the N card
                (App.pinnedCoins[n], App.pinnedCoins[n-1]) = (App.pinnedCoins[n-1], App.pinnedCoins[n]);
                (vm.PriceCards[n], vm.PriceCards[n-1]) = (vm.PriceCards[n-1], vm.PriceCards[n]);
                (vm.VolumeCards[n], vm.VolumeCards[n-1]) = (vm.VolumeCards[n-1], vm.VolumeCards[n]);

                /// Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        /// #######################################################################################
        private async void TimeRangeButtons_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            timeSpan = ((UserControls.TimeRangeRadioButtons)sender)?.TimeSpan ?? timeSpan;
            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];

            await UpdateAllCards();
        }
    }
}

