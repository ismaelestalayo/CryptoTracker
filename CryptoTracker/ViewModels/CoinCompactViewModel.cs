using CryptoTracker.Model;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoTracker.ViewModels {
	public class CoinCompactViewModel : ObservableObject {
		private CoinCompactModel _coinInfo = new CoinCompactModel();

		public CoinCompactModel CoinInfo {
			get => _coinInfo;
			set => SetProperty(ref _coinInfo, value);
		}
	}
}
