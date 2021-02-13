using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace CryptoTracker.ViewModels {
	public class CoinCompactViewModel : ObservableObject {

		private HomeCard card = new HomeCard();
		public HomeCard Card {
			get => card;
			set => SetProperty(ref card, value);
		}
	}
}
