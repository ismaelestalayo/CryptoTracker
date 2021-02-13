using CryptoTracker.Helpers;
using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CryptoTracker.APIs.CryptoCompare;


namespace CryptoTracker.Views {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CoinCompact : Page {

		public CoinCompact() {
			this.InitializeComponent();
		}

        protected override void OnNavigatedTo(NavigationEventArgs e) {
			/// Page title
			var crypto = e.Parameter?.ToString().ToUpper(CultureInfo.InvariantCulture) ?? "NULL";
			viewModel.Card.Info.Name = crypto;

			UpdateValues();
		}

		/// #########################################################################################
		private async void UpdateValues() {
			var crypto = viewModel.Card.Info.Name;

			/// Get current price
			viewModel.Card.Info.Price = await GetPriceAsync(crypto);

			/// Get historic values
			var histo = await GetHistoricAsync(crypto, "minute", 60);

			var chartData = new List<ChartPoint>();
			foreach (var h in histo)
				chartData.Add(new ChartPoint() {
					Date = h.DateTime,
					Value = h.Average
				});
			viewModel.Card.Chart.ChartData = chartData;

			/// Calculate diff based on historic prices
			double oldestPrice = histo[0].Average;
			double newestPrice = histo[histo.Count - 1].Average;
			double diff = (double)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

			viewModel.Card.Info.Diff = diff;

			SolidColorBrush brush;
			if (diff > 0)
				brush = ((SolidColorBrush)Application.Current.Resources["pastelGreen"]);
			else
				brush = ((SolidColorBrush)Application.Current.Resources["pastelRed"]);

			viewModel.Card.Chart.ChartStroke = brush;
			var color = brush.Color;
			viewModel.Card.Chart.ChartFill1 = Color.FromArgb(64, color.R, color.G, color.B);
			viewModel.Card.Chart.ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);

			viewModel.Card.Chart.PricesMinMax = GraphHelper.GetMinMaxOfArray(chartData.Select(d => d.Value).ToList());
		}

        private async void FullScreen_btn_click(object sender, RoutedEventArgs e) {
			var view = ApplicationView.GetForCurrentView();

			await view.TryEnterViewModeAsync(ApplicationViewMode.Default);
			Frame.Navigate(typeof(CoinDetails), viewModel.Card.Info.Name);
		}
	}
}
