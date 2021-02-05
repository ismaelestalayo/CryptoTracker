using CryptoTracker.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTracker.ViewModels {
	public class CoinCompactViewModel : ObservableObject {
		private CoinCard _coinCard = new CoinCard();

		public CoinCard CoinCard {
			get => _coinCard;
			set => SetProperty(ref _coinCard, value);
		}
	}
}
