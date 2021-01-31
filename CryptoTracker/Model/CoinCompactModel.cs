using CryptoTracker.Helpers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Model {
	internal class CoinCompactModel : ObservableObject {
		internal string Crypto { get; set; }
		internal string Currency { get; set; }

		private double _currentPrice = 0;
		internal double CurrentPrice {
			get => _currentPrice;
			set => SetProperty(ref _currentPrice, value);
		}

		private double _currentDiff = 0;
		internal double CurrentDiff {
			get => _currentDiff;
			set => SetProperty(ref _currentDiff, value);
		}

		private string _currentDiffArrow = "▲";
		internal string CurrentDiffArrow {
			get => _currentDiffArrow;
			set => SetProperty(ref _currentDiffArrow, value);
		}

		private Brush _diffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
		internal Brush DiffFG {
			get => _diffFG;
			set => SetProperty(ref _diffFG, value);
		}

		private Brush _chartStroke = (SolidColorBrush)App.Current.Resources["pastelGreen"];
		internal Brush ChartStroke {
			get => _chartStroke;
			set => SetProperty(ref _chartStroke, value);
		}


		private Color _chartFill1 = ((SolidColorBrush)App.Current.Resources["pastelGreen"]).Color;
		private Color _chartFill2 = ((SolidColorBrush)App.Current.Resources["pastelGreen"]).Color;

		internal Color ChartFill1 {
			get => _chartFill1;
			set => SetProperty(ref _chartFill1, value);
		}
		internal Color ChartFill2 {
			get => _chartFill2;
			set => SetProperty(ref _chartFill2, value);
		}

		private (float Min, float Max) _historicMinMax = (0, 100);
        internal (float Min, float Max) HistoricMinMax {
            get => _historicMinMax;
            set => SetProperty(ref _historicMinMax, value);
        }

		private List<ChartData> _historicValues = new List<ChartData>();
		internal List<ChartData> HistoricValues {
			get => _historicValues;
			set => SetProperty(ref _historicValues, value);
		}

		internal string LogoSource { get; set; }
	}
}
