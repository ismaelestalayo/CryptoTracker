using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTracker.ViewModels {
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

        public void InAppNotification(string title, string message = "", bool temporary = true) {
            var tuple = new Tuple<string, string, bool>(title, message, temporary);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
