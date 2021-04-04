using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAlerts : Page {

        private static LocalSettings localSettings = new LocalSettings();
        private static string currency = localSettings.Get<string>(UserSettings.Currency);
        private static string currencySym = Currencies.GetCurrencySymbol(currency);
        private ObservableCollection<Alert> alerts { get; set; }

        public SettingsAlerts() {
            this.InitializeComponent();
        }

        private new async void Loaded(object sender, RoutedEventArgs e) {
            alerts = await LocalStorageHelper.ReadObject<ObservableCollection<Alert>>(UserStorage.Alerts);
        }

        private new void Unloaded(object sender, RoutedEventArgs e) {
            LocalStorageHelper.SaveObject(UserStorage.Alerts, alerts);
        }


        // ###############################################################################################
        private async Task CreateAlert(string crypto, string mode, double threshold) {
            alerts.Add(new Alert() {
                Crypto = crypto,
                Currency = currency,
                CurrencySymbol = currencySym,
                Enabled = true,
                Id = alerts.Count,
                Mode = mode,
                Threshold = threshold
            });
            localSettings.Set(UserStorage.Alerts, alerts);
        }

        private void Delete_alert(object sender, RoutedEventArgs e) {
            var z = sender;
        }
    }
}
