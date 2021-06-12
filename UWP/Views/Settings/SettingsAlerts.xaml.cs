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
        }

        private new async void Loaded(object sender, RoutedEventArgs e) {
            var alerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            vm.Alerts = new ObservableCollection<Alert>(alerts);
            vm.CvsSource = from alert in vm.Alerts group alert by alert.Crypto;
        }

        private new void Unloaded(object sender, RoutedEventArgs e) {
            LocalStorageHelper.SaveObject(UserStorage.Alerts, vm.Alerts);
        }


        // ###############################################################################################
        private void Delete_alert(object sender, RoutedEventArgs e) {
            var alert = ((FrameworkElement)sender).DataContext as Alert;
            vm.Alerts.Remove(alert);
            vm.CvsSource = from a in vm.Alerts group a by a.Crypto;
        }
    }
}
