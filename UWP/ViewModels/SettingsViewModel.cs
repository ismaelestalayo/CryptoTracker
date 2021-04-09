using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Core.Constants;
using UWP.Models;

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

		private string autoRefresh = App._LocalSettings.Get<string>(UserSettings.AutoRefresh);
		public string AutoRefresh {
			get => autoRefresh;
			set {
				if (SetProperty(ref autoRefresh, value))
					App._LocalSettings.Set(UserSettings.AutoRefresh, autoRefresh);
			}
		}

		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
