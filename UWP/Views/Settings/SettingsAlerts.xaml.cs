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


        public SettingsAlerts() {
            this.InitializeComponent();
        }

        private new async void Loaded(object sender, RoutedEventArgs e) {
            vm.Alerts = await LocalStorageHelper.ReadObject<ObservableCollection<Alert>>(UserStorage.Alerts);
        }

        private new void Unloaded(object sender, RoutedEventArgs e) {
            LocalStorageHelper.SaveObject(UserStorage.Alerts, vm.Alerts);
        }


        // ###############################################################################################
        private async Task CreateAlert(string crypto, string mode, double threshold) {
            vm.Alerts.Add(new Alert() {
                Crypto = crypto,
                Currency = currency,
                CurrencySymbol = currencySym,
                Enabled = true,
                Id = vm.Alerts.Count,
                Mode = 0,
                Threshold = threshold
            });
            localSettings.Set(UserStorage.Alerts, vm.Alerts);
        }

        private void Delete_alert(object sender, RoutedEventArgs e) {
            var alert = ((FrameworkElement)sender).DataContext as Alert;
            vm.Alerts.Remove(alert);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            vm.Alerts.Add(
                new Alert() {
                    Crypto = "ETH",
                    Enabled = true,
                    Id = 0,
                    Mode = 0,
                    Threshold = 1720
                });
            vm.Alerts.Add(
                new Alert() {
                    Crypto = "BTC",
                    Enabled = true,
                    Id = 1,
                    Mode = 0,
                    Threshold = 60000
                });
        }
    }
}
