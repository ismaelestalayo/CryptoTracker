using System;
using System.Collections.Generic;
using System.Linq;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Shared.Constants;
using UWP.UserControls;
using UWP.ViewModels;
using Windows.System.Threading;
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

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			vm.Chart.ChartStroke = ColorConstants.GetBrush(vm.Info.Name);
		}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
			var type = (e.Parameter.GetType()).Name;

			var coinDetailsVM = (CoinDetailsViewModel)e.Parameter;
			vm.CoinDetailsVM = coinDetailsVM;
			vm.Chart = coinDetailsVM.Chart;
			vm.Info = coinDetailsVM.Coin;
			vm.Chart.TimeSpan = vm.Chart.TimeSpan;
			if (!timeSpans.Contains(vm.Chart.TimeSpan)) {
				(timeUnit, limit, aggregate) = GraphHelper.TimeSpanParser[timeSpan];
				vm.Chart.TimeSpan = timeSpan;
				UpdateValues();
			}
			else
				timeSpan = vm.Chart.TimeSpan;


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
			var diff = newestPrice - oldestPrice;
			vm.Info.Diff = diff;

			var brush = (diff > 0) ?
				(SolidColorBrush)Application.Current.Resources["pastelGreen"] :
				(SolidColorBrush)Application.Current.Resources["pastelRed"];

			vm.Chart.ChartStroke = brush;

			var MinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
			vm.Chart.PricesMinMax = GraphHelper.OffsetMinMaxForChart(MinMax.Min, MinMax.Max, 0.25);
		}

        private async void FullScreen_btn_click(object sender, RoutedEventArgs e) {
			var view = ApplicationView.GetForCurrentView();

			await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
			Frame.Navigate(typeof(CoinDetails), vm.CoinDetailsVM);
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
