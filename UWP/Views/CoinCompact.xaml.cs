using UWP.Helpers;
using UWP.Models;
using UWP.UserControls;
using UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static UWP.APIs.CryptoCompare;


namespace UWP.Views {
	public sealed partial class CoinCompact : Page {
		/// Variables to get historic
		private static int limit = 168;
		private static int aggregate = 1;
		private static string timeSpan = "4h";
		private static string timeUnit = "minute";

		private List<string> timeSpans = new List<string>{"1h", "4h", "1d"};

		/// Timer for auto-refresh
		private static ThreadPoolTimer PeriodicTimer;

		public CoinCompact() {
			this.InitializeComponent();
		}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
			var type = (e.Parameter.GetType()).Name;

			switch (type) {
				case nameof(CoinDetailsViewModel):
					vm.Chart = ((CoinDetailsViewModel)e.Parameter).Chart;
					vm.Info = ((CoinDetailsViewModel)e.Parameter).Coin;
					vm.Chart.TimeSpan = vm.Chart.TimeSpan;
					if (!timeSpans.Contains(vm.Chart.TimeSpan)) {
						(timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
						vm.Chart.TimeSpan = timeSpan;
						UpdateValues();
					}
					else
						timeSpan = vm.Chart.TimeSpan;

					break;
				default:
				case "string":
					var crypto = e.Parameter?.ToString().ToUpper(CultureInfo.InvariantCulture) ?? "NULL";
					vm.Info.Name = crypto;
					UpdateValues();
					break;
            }

			/// Create the auto-refresh timer
			TimeSpan period = TimeSpan.FromSeconds(30);
			PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
					TimeRangeButtons_Tapped(null, null);
				});
			}, period);
		}

		private void Page_Unloaded(object sender, RoutedEventArgs e) {
			PeriodicTimer?.Cancel();
		}

		/// #########################################################################################
		private async void UpdateValues() {
			var crypto = vm.Info.Name;

			/// Get current price
			vm.Info.Price = await GetPriceAsync(crypto);

			/// Get historic values
			var histo = await GetHistoricAsync(crypto, timeUnit, limit, aggregate);

			var chartData = new List<ChartPoint>();
			foreach (var h in histo)
				chartData.Add(new ChartPoint() {
					Date = h.DateTime,
					Value = h.Average
				});
			vm.Chart.ChartData = chartData;

			/// Calculate diff based on historic prices
			double oldestPrice = histo[0].Average;
			double newestPrice = histo[histo.Count - 1].Average;
			double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

			vm.Info.Diff = diff;

			SolidColorBrush brush;
			if (diff > 0)
				brush = ((SolidColorBrush)Application.Current.Resources["pastelGreen"]);
			else
				brush = ((SolidColorBrush)Application.Current.Resources["pastelRed"]);

			vm.Chart.ChartStroke = brush;
			var color = brush.Color;
			vm.Chart.ChartFill1 = Color.FromArgb(64, color.R, color.G, color.B);
			vm.Chart.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);

			var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
			vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max, 0.25);
		}

        private async void FullScreen_btn_click(object sender, RoutedEventArgs e) {
			var view = ApplicationView.GetForCurrentView();

			await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
			Frame.Navigate(typeof(CoinDetails), vm);
		}

        private void TimeRangeButtons_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
			if (sender != null)
				timeSpan = ((TimeRangeRadioButtons)sender).TimeSpan;

			(timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
			vm.Chart.TimeSpan = timeSpan;

			UpdateValues();
		}
    }
}
