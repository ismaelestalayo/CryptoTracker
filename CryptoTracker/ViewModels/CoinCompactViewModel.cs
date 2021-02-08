using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CryptoTracker.ViewModels {
	public class CoinCompactViewModel : ObservableObject {

		private CoinCard _coinCard = new CoinCard();
		public CoinCard CoinCard {
			get => _coinCard;
			set => SetProperty(ref _coinCard, value);
		}
	}
}
