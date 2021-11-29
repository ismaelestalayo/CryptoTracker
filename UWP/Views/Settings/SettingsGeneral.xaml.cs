using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
            vm.StartupPage = App._LocalSettings.Get<string>(UserSettings.StartupPage).Replace("/", "");
            vm.Timespan = App._LocalSettings.Get<string>(UserSettings.Timespan);
        }

        // ###############################################################################################
        private async void SetAllPurchasesCurrency_Click(object sender, RoutedEventArgs e) {
            var portfolio = await PortfolioHelper.GetPortfolio();
            foreach (var purchase in portfolio)
                purchase.Currency = vm.Currency;
            await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, portfolio);
            var openedpopups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in openedpopups) {
                if (popup.Child is ContentDialog)
                    ((ContentDialog)popup.Child).Hide();
            }
            vm.InAppNotification("Currency of all purchases overrided.", "Please refresh the portfolio.");
        }

        // ###############################################################################################
        private void StartupPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => App._LocalSettings.Set(UserSettings.StartupPage, $"/{vm.StartupPage}");

        private void Timespan_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => App._LocalSettings.Set(UserSettings.Timespan, vm.Timespan);
    }
}
