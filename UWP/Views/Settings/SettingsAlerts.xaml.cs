using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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


        public SettingsAlerts() {
            InitializeComponent();
            Loaded += SettingsAlerts_Loaded;
            Unloaded += SettingsAlerts_Unloaded;
        }

        private async void SettingsAlerts_Loaded(object sender, RoutedEventArgs e) {
            var alerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            vm.Alerts = new ObservableCollection<Alert>(alerts);
            vm.CvsSource = from alert in vm.Alerts group alert by alert.Crypto;
        }

        private async void SettingsAlerts_Unloaded(object sender, RoutedEventArgs e) {
            await LocalStorageHelper.SaveObject(UserStorage.Alerts, vm.Alerts);
        }

        private void AlertsTest_Click(object sender, RoutedEventArgs e) {
            UWP.Background.ToastGenerator.SendAlert("Test", "Notification demo", "");
        }
    }
}
