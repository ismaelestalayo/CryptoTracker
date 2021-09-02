using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsGeneral : Page {

        private Dictionary<string, string> AvailableCurrencies = Currencies.CurrencySymbol;

        public SettingsGeneral() {
            InitializeComponent();
            Loaded += SettingsGeneral_Loaded;
        }

        private void SettingsGeneral_Loaded(object sender, RoutedEventArgs e) {
            vm.AutoRefresh = App._LocalSettings.Get<string>(UserSettings.AutoRefresh);
            vm.Currency = App._LocalSettings.Get<string>(UserSettings.Currency);
            startupPage.PlaceholderText = App._LocalSettings.Get<string>(UserSettings.StartupPage).Replace("/", "");
        }

        // ###############################################################################################
        private void startupPage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var startupPage = ((ComboBox)sender).SelectedItem.ToString();
            App._LocalSettings.Set(UserSettings.StartupPage, $"/{startupPage}");
        }

    }
}
