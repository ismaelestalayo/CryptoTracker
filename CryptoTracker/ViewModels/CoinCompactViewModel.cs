using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CryptoTracker.ViewModels {
	public class CoinCompactViewModel : ObservableObject {

		private Coin info = new Coin();
		public Coin Info {
			get => info;
			set => SetProperty(ref info, value);
		}

		private ChartModel chart = new ChartModel();
		public ChartModel Chart {
			get => chart;
			set => SetProperty(ref chart, value);
		}
	}
}
