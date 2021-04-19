using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using UWP.Models;
using UWP.Services;

namespace UWP.ViewModels {
    public class Top100ViewModel : ObservableRecipient {

        private GlobalStats globalStats = new GlobalStats();
        public GlobalStats GlobalStats {
            get => globalStats;
            set => SetProperty(ref globalStats, value);
        }

        private List<Top100card> top100cards = Enumerable.Repeat(new Top100card(), 30).ToList();
        public List<Top100card> Top100cards {
            get => top100cards;
            set => SetProperty(ref top100cards, value);
        }

        private List<CoinMarket> coinMarket;
        public List<CoinMarket> CoinMarket {
            get => coinMarket;
            set => SetProperty(ref coinMarket, value);
        }

        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
