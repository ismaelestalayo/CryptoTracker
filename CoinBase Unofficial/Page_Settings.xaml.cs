using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CoinBase {
    public sealed partial class Page_Settings : Page {
        public Page_Settings() {
            this.InitializeComponent();

            if (App.localSettings.Values["Theme"].Equals("Dark"))
                DarkRadioBtn.IsChecked = true;
            else if (App.localSettings.Values["Theme"].Equals("Light"))
                LightRadioBtn.IsChecked = true;

        }

        private void DarkTheme(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Theme"] = "Dark";
            ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
        }

        private void LightTheme(object sender, RoutedEventArgs e) {
            App.localSettings.Values["Theme"] = "Light";
            ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
        }
    }
}
