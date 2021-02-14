using CryptoTracker.Helpers;
using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CryptoTracker.ViewModels {
	class CoinDetailsViewModel : ObservableObject {

		private CoinData _coinInfo = new CoinData();
		public CoinData CoinInfo {
			get => _coinInfo;
			set => SetProperty(ref _coinInfo, value);
		}

		private Coin coin = new Coin();
		public Coin Coin {
			get => coin;
			set => SetProperty(ref coin, value);
		}

		private ChartModel chart = new ChartModel();
		public ChartModel Chart {
			get => chart;
			set => SetProperty(ref chart, value);
		}
	}
}
