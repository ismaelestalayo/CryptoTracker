using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Core.Constants;
using UWP.Models;
using UWP.Shared.Constants;
using Windows.ApplicationModel;

namespace UWP.ViewModels {
    partial class SettingsViewModel : ObservableRecipient {

        [ObservableProperty]
        private ObservableCollection<PurchaseModel> purchaseList = new ObservableCollection<PurchaseModel>();

        [ObservableProperty]
        private ObservableCollection<Alert> alerts = new ObservableCollection<Alert>();


        [ObservableProperty]
        private object cvsSource;


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

        [ObservableProperty]
        private string timespan;

        [ObservableProperty]
        private string startupPage;

        [ObservableProperty]
        private bool openInLogin = false;

        [ObservableProperty]
        private bool canOpenInLogin = false;


        /// #######################################################################################
        ///  Notifications
        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
