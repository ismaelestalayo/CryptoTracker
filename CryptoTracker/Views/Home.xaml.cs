using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
    public sealed partial class Home : Page {
        /// Variables to get historic
        private static int limit = 168;
        private static int aggregate = 1;
        private static string timeSpan = "1w";
        private static string timeUnit = "hour";

        /// Timer for auto-refresh
        private static ThreadPoolTimer PeriodicTimer;

        private bool AppIsActive = false;

        public Home() {
            this.InitializeComponent();
            Window.Current.VisibilityChanged += WindowVisibilityChangedEventHandler;
        }

        void WindowVisibilityChangedEventHandler(object sender, VisibilityChangedEventArgs e) {
            // Perform operations that should take place when the application becomes visible rather than
            // when it is prelaunched, such as building a what's new feed
            AppIsActive = e.Visible;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            if (!AppIsActive)
                return;
            /// First keep an updated list of coins
            await App.GetCoinList();

            InitHome();

            /// Create the auto-refresh timer
            TimeSpan period = TimeSpan.FromSeconds(30);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    if (timeUnit == "minute")
                        TimeRangeButtons_Tapped(null, null);
                });
            }, period);
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

            for (int i = 0; i < App.pinnedCoins.Count; i++)
                await UpdateCard(i);
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

                /// Get price
                vm.PriceCards[i].Info.Price = await CryptoCompare.GetPriceAsync(crypto);

                /// Save the current timeSpan for navigating to another page
                vm.PriceCards[i].Chart.TimeSpan = timeSpan;

                /// Colors
                var brush = vm.PriceCards[i].Chart.ChartStroke;
                if (Application.Current.Resources.ContainsKey(crypto.ToUpper() + "_colorT"))
                    brush = (SolidColorBrush)Application.Current.Resources[crypto.ToUpper() + "_color"];
                vm.PriceCards[i].Chart.ChartStroke = brush;

                /// Get Historic and create List of ChartData for the chart
                var histo = await CryptoCompare.GetHistoricAsync(crypto, timeUnit, limit, aggregate);
                var chartData = new List<ChartPoint>();
                foreach (var h in histo) {
                    chartData.Add(new ChartPoint() {
                        Date = h.DateTime,
                        Value = h.Average,
                        Volume = h.volumefrom
                    });
                }
                vm.PriceCards[i].Chart.ChartData = chartData;
                vm.VolumeCards[i].Chart.ChartData = chartData;
                var temp = GraphHelper.AdjustLinearAxis(new ChartStyling(), timeSpan);
                vm.PriceCards[i].Chart.LabelFormat = temp.LabelFormat;
                vm.PriceCards[i].Chart.Minimum = temp.Minimum;
                vm.PriceCards[i].Chart.MajorStepUnit = temp.MajorStepUnit;
                vm.PriceCards[i].Chart.MajorStep = temp.MajorStep;

                /// Calculate min-max to adjust axis
                var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
                vm.PriceCards[i].Chart.PricesMinMax = MinMax;

                /// Calculate the price difference
                double oldestPrice = histo[0].Average;
                double newestPrice = histo[histo.Count - 1].Average;
                double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);
                vm.PriceCards[i].Info.Diff = diff;

                /// Sum total volume from historic
                double total = 0;
                histo.ForEach(x => total += x.volumeto);
                vm.VolumeCards[i].Info.Volume = NumberHelper.AddUnitPrefix(total) + App.currencySymbol;

                /// Show that loading is done
                vm.PriceCards[i].Info.IsLoading = false;
            } catch (Exception) {  }
            
        }

        /// #########################################################################################
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

        private async void TimeRangeButtons_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            timeSpan = ((UserControls.TimeRangeRadioButtons)sender).TimeSpan;
            (timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];

            await UpdateAllCards();
        }
    }
}

