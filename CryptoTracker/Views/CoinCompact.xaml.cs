using CryptoTracker.Helpers;
using CryptoTracker.Model;
using CryptoTracker.APIs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CryptoTracker.APIs.CryptoCompare;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CoinCompact : Page {

		public CoinCompact() {
			this.InitializeComponent();
		}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
			// Page title
			var crypto = e.Parameter?.ToString().ToUpper(CultureInfo.InvariantCulture) ?? "NULL";
			viewModel.CoinInfo.Crypto = crypto;

			// Crypto icon
			var iconExists = File.Exists("Assets/Icons/icon" + crypto + ".png");
			viewModel.CoinInfo.LogoSource = iconExists ? "ms-appx:///Assets/Icons/icon" + crypto + ".png" : "ms-appx:///Assets/Icons/iconNULL.png";

			UpdateValues();
		}

		/// #########################################################################################
		private async void UpdateValues() {
			var crypto = viewModel.CoinInfo.Crypto;

			/// Get current price
			viewModel.CoinInfo.CurrentPrice = await CryptoCompare.GetPriceAsync(crypto);

			/// Get historic values
			var histo = await CryptoCompare.GetHistoricAsync(crypto, "minute", 60);

			var chartData = new List<ChartData>();
			foreach (var h in histo)
				chartData.Add(new ChartData() {
					Date = h.DateTime,
					Value = h.Average
				});

			/// Calculate diff based on historic prices
			double oldestPrice = histo[0].Average;
			double newestPrice = histo[histo.Count - 1].Average;
			double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

			viewModel.CoinInfo.CurrentDiff = diff;

			if (diff > 0) {
				viewModel.CoinInfo.CurrentDiff = diff;
				viewModel.CoinInfo.CurrentDiffArrow = "▲";
				var brush = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
				var color = brush.Color;
				viewModel.CoinInfo.DiffFG = brush;
				viewModel.CoinInfo.ChartStroke = brush;
				viewModel.CoinInfo.ChartFill1 = Color.FromArgb(62, color.R, color.G, color.B);
				viewModel.CoinInfo.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
			}
			else {
				viewModel.CoinInfo.CurrentDiff = Math.Abs(diff);
				viewModel.CoinInfo.CurrentDiffArrow = "▼";
				var brush = (SolidColorBrush)Application.Current.Resources["pastelRed"];
				var color = brush.Color;
				viewModel.CoinInfo.DiffFG = brush;
				viewModel.CoinInfo.ChartStroke = brush;
				viewModel.CoinInfo.ChartFill1 = Color.FromArgb(62, color.R, color.G, color.B);
				viewModel.CoinInfo.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
			}


			/// Create the chart
			var series = (SplineAreaSeries)HistoricChart.Series[0];
			series.ItemsSource = chartData;

			viewModel.CoinInfo.HistoricMinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
		}

        private async void FullScreen_btn_click(object sender, RoutedEventArgs e) {
			var view = ApplicationView.GetForCurrentView();

			await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
			Frame.Navigate(typeof(CoinDetails), viewModel.CoinInfo.Crypto);
		}
	}
}
