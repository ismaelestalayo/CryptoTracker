using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CoinBase {
    public sealed partial class Page_Settings : Page {
        public Page_Settings() {
            this.InitializeComponent();

            ThemeSwitcher.IsOn = App.localSettings.Values["Theme"].Equals("Dark");
            CoinSwitcher.IsOn  = App.localSettings.Values["Coin"].Equals("USD");

        }

        private void DarkTheme(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Theme"] = "Dark";
            ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
        }

        private void LightTheme(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Theme"] = "Light";
            ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
        }

        private void EUR(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Coin"] = "EUR";
            App.coin = "EUR";
        }

        private void USD(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Coin"] = "USD";
            App.coin = "USD";
        }

        private void ThemeToogled(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch != null) {
                if (toggleSwitch.IsOn == true) {
                    App.localSettings.Values["Theme"] = "Dark";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                } else {
                    App.localSettings.Values["Theme"] = "Light";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                }
            }
        }

        private void CoinToogled(object sender, RoutedEventArgs e) {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            if (toggleSwitch != null) {
                if (toggleSwitch.IsOn == true) {
                    App.localSettings.Values["Coin"] = "USD";
                    App.coin = "USD";
                } else {
                    App.localSettings.Values["Coin"] = "EUR";
                    App.coin = "EUR";
                }
            }
        }
    }
}
