using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;
using UWP.Services;

namespace UWP.ViewModels {
    public partial class CoinsViewModel : ObservableRecipient {

        [ObservableProperty]
        private GlobalStats globalStats = new GlobalStats();

        [ObservableProperty]
        private ObservableCollection<CoinMarket> coinMarket = new ObservableCollection<CoinMarket>();


        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
