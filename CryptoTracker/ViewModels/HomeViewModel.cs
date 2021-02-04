using CryptoTracker.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CryptoTracker.ViewModels {
	public class HomeViewModel : ObservableObject {
		private ObservableCollection<CoinCard> _coinCards = new ObservableCollection<CoinCard>();

		public ObservableCollection<CoinCard> CoinCards {
			get => _coinCards;
			set => SetProperty(ref _coinCards, value);
		}
	}
}
