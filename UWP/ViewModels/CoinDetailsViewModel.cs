using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;
using static UWP.APIs.CoinGecko;

namespace UWP.ViewModels {
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

        private ObservableCollection<PurchaseModel> purchases;
        public ObservableCollection<PurchaseModel> Purchases {
            get => purchases;
            set => SetProperty(ref purchases, value);
        }

        private bool showCandles = false;
        public bool ShowCandles {
            get => showCandles;
            set => SetProperty(ref showCandles, value);
        }

        /// #############################################################################
        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
