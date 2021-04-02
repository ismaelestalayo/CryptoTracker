using UWP.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System;
using Microsoft.Toolkit.Mvvm.Messaging;

namespace UWP.ViewModels {
	public class HomeViewModel : ObservableRecipient {

		private ObservableCollection<HomeCard> priceCards = new ObservableCollection<HomeCard>();
		public ObservableCollection<HomeCard> PriceCards {
			get => priceCards;
			set => SetProperty(ref priceCards, value);
		}

		private ObservableCollection<HomeCard> volumeCards = new ObservableCollection<HomeCard>();
		public ObservableCollection<HomeCard> VolumeCards {
			get => volumeCards;
			set => SetProperty(ref volumeCards, value);
		}

		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
