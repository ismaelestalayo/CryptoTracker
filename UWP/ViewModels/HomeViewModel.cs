using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;

namespace UWP.ViewModels {
    public class HomeViewModel : ObservableRecipient {

		private ObservableCollection<HomeCard> priceCards = new ObservableCollection<HomeCard>();
		public ObservableCollection<HomeCard> PriceCards {
			get => priceCards;
			set => SetProperty(ref priceCards, value);
		}

		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
