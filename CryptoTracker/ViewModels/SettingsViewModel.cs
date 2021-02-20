using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.ObjectModel;

namespace CryptoTracker.ViewModels {
    class SettingsViewModel : ObservableRecipient {
		private ObservableCollection<PurchaseModel> purchaseList = new ObservableCollection<PurchaseModel>();
		public ObservableCollection<PurchaseModel> PurchaseList {
			get => purchaseList;
			set => SetProperty(ref purchaseList, value);
		}

		public void InAppNotification(string severity, string title, string message) {
			var tuple = new Tuple<string, string, string>(severity, title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}

		internal void Test() {
			Messenger.Send(new PortfolioMessage(PurchaseList));
		}
	}

	public sealed class PortfolioMessage : ValueChangedMessage<ObservableCollection<PurchaseModel>> {
		public PortfolioMessage(ObservableCollection<PurchaseModel> portfolio) : base(portfolio) {
		}
	}

}
