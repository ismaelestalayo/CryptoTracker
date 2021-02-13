using CryptoTracker.Helpers;
using CryptoTracker.Models;
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

		private ChartModel chartModel = new ChartModel();
		public ChartModel ChartModel {
			get => chartModel;
			set => SetProperty(ref chartModel, value);
		}
	}
}
