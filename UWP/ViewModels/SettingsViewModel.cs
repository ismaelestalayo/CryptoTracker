using UWP.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace UWP.ViewModels {
    class SettingsViewModel : ObservableRecipient {
		private ObservableCollection<PurchaseModel> purchaseList = new ObservableCollection<PurchaseModel>();
		public ObservableCollection<PurchaseModel> PurchaseList {
			get => purchaseList;
			set => SetProperty(ref purchaseList, value);
		}

		private ObservableCollection<Alert> alerts = new ObservableCollection<Alert>();
		public ObservableCollection<Alert> Alerts {
			get => alerts;
			set => SetProperty(ref alerts, value);
		}

		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
