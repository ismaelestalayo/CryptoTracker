using CryptoTracker.Helpers;
using CryptoTracker.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CryptoTracker.ViewModels {
	class CoinDetailsViewModel : ObservableObject {

		private CoinCard _coinCard = new CoinCard();
		public CoinCard CoinCard {
			get => _coinCard;
			set => SetProperty(ref _coinCard, value);
		}

		private CoinData _coinInfo = new CoinData();
		public CoinData CoinInfo {
			get => _coinInfo;
			set => SetProperty(ref _coinInfo, value);
		}

		private ChartStyling _chartSyle = new ChartStyling();
		public ChartStyling ChartSyle {
			get => _chartSyle;
			set => SetProperty(ref _chartSyle, value);
		}
	}
}
