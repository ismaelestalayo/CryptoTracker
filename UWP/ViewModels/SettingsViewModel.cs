using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Core.Constants;
using UWP.Models;
using UWP.Shared.Constants;

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

		private string autoRefresh = "";
		public string AutoRefresh {
			get => autoRefresh;
			set {
				if (SetProperty(ref autoRefresh, value))
					App._LocalSettings.Set(UserSettings.AutoRefresh, autoRefresh);
			}
		}

		private string currency = "";
		public string Currency {
			get => currency;
			set {
				if (SetProperty(ref currency, value)) {
					/// Update LocalSettings' Currency and symbol
					App._LocalSettings.Set(UserSettings.Currency, currency);
					var currencySym = Currencies.GetCurrencySymbol(currency);
					App._LocalSettings.Set(UserSettings.CurrencySymbol, currencySym);
					/// Update App values as well
					App.currency = currency;
					App.currencySymbol = currencySym;
				}
			}
		}

		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
