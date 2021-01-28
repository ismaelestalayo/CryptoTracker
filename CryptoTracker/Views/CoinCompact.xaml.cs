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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CoinCompact : Page {

		internal CoinCompactModel CoinModel { get; set; }

		public CoinCompact() {
			this.InitializeComponent();

			CoinModel = new CoinCompactModel() {
				Currency = App.coinSymbol
			};
		}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
			// Page title
			var crypto = e.Parameter?.ToString().ToUpper(CultureInfo.InvariantCulture) ?? "NULL";
			CoinModel.Crypto = crypto;

			// Crypto icon
			var iconExists = File.Exists("Assets/Icons/icon" + crypto + ".png");
			CoinModel.LogoSource = iconExists ? "ms-appx:///Assets/Icons/icon" + crypto + ".png" : "ms-appx:///Assets/Icons/iconNULL.png";

			UpdateValues();
		}

		private async void UpdateValues() {
			var crypto = CoinModel.Crypto;

			// Current price
			CoinModel.CurrentPrice = CryptoCompare.GetPrice(crypto, "defaultMarket");

			// Historic values
			App.GetHisto(crypto, "minute", 60);
			List<ChartData> histo = new List<ChartData>();
			for (int i = 0; i < App.historic.Count; ++i) {
				histo.Add(new ChartData {
					Date = App.historic[i].DateTime,
					Value = (App.historic[i].Low + App.historic[i].High) / 2,
					Low = App.historic[i].Low,
					High = App.historic[i].High,
					Open = App.historic[i].Open,
					Close = App.historic[i].Close,
					Volume = App.historic[i].Volumefrom
				});
			}

			// Calculate diff based on historic prices
			double oldestPrice = App.historic[0].Close;
			double newestPrice = App.historic[App.historic.Count - 1].Close;
			double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

			CoinModel.CurrentDiff = diff;

			if (diff > 0) {
				CoinModel.CurrentDiff = diff;
				CoinModel.CurrentDiffArrow = "▲";
				var brush = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
				var color = brush.Color;
				CoinModel.DiffFG = brush;
				CoinModel.ChartStroke = brush;
				CoinModel.ChartFill1 = Color.FromArgb(62, color.R, color.G, color.B);
				CoinModel.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
			}
			else {
				CoinModel.CurrentDiff = Math.Abs(diff);
				CoinModel.CurrentDiffArrow = "▼";
				var brush = (SolidColorBrush)Application.Current.Resources["pastelRed"];
				var color = brush.Color;
				CoinModel.DiffFG = brush;
				CoinModel.ChartStroke = brush;
				CoinModel.ChartFill1 = Color.FromArgb(62, color.R, color.G, color.B);
				CoinModel.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
			}


			// Create the chart
			var series = (SplineAreaSeries)HistoricChart.Series[0];
			series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
			series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
			series.ItemsSource = histo;

			var min = GraphHelper.GetMinimumOfArray(histo.Select(d => d.Value).ToList());
			var max = GraphHelper.GetMaximumOfArray(histo.Select(d => d.Value).ToList());
			CoinModel.HistoricMin = min - (float)(min * 0.02);
			CoinModel.HistoricMax = max + (float)(max * 0.02);
		}

        private async void FullScreen_btn_click(object sender, RoutedEventArgs e) {
			var view = ApplicationView.GetForCurrentView();

			await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
			Frame.Navigate(typeof(CoinDetails), CoinModel.Crypto);
		}
	}
}
