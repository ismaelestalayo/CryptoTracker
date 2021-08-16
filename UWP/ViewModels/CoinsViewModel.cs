using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;
using UWP.Services;

namespace UWP.ViewModels {
    public class CoinsCacheViewModel : ObservableRecipient {

        private GlobalStats globalStats = new GlobalStats();
        public GlobalStats GlobalStats {
            get => globalStats;
            set => SetProperty(ref globalStats, value);
        }

        private ObservableCollection<CoinMarket> coinMarket = new ObservableCollection<CoinMarket>();
        public ObservableCollection<CoinMarket> CoinMarket {
            get => coinMarket;
            set => SetProperty(ref coinMarket, value);
        }

        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
