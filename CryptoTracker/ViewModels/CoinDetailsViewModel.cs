using CryptoTracker.Helpers;
using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;

namespace CryptoTracker.ViewModels {
	class CoinDetailsViewModel : ObservableRecipient {

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

		public void InAppNotification(string title, string message = "", bool temporary = true) {
			var tuple = new Tuple<string, string, bool>(title, message, temporary);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
