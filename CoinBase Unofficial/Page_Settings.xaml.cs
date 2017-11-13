using System;
using System.Reflection;
using Windows.ApplicationModel.Email;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CryptoTracker {
    public sealed partial class Page_Settings : Page {

        private string v;

        public Page_Settings() {
            this.InitializeComponent();

            var assembly = typeof(App).GetTypeInfo().Assembly;
            v = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            VersionTextBlock.Text = "Version: " + v;

            ThemeSwitcher.IsOn = App.localSettings.Values["Theme"].Equals("Dark");

            switch (App.localSettings.Values["Coin"]) {
                case "EUR":
                    EUR.IsSelected = true;
                    break;
                case "USD":
                    USD.IsSelected = true;
                    break;
                case "CAD":
                    CAD.IsSelected = true;
                    break;
                case "MXN":
                    MXN.IsSelected = true;
                    break;
                case "CNY":
                    CNY.IsSelected = true;
                    break;
            }
            CoinComboBox.PlaceholderText = App.localSettings.Values["Coin"].ToString();

            //Show feedback button
            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported()) {
                this.feedbackButton.IsEnabled = true;
            } else {
                this.feedbackButton.IsEnabled = false;
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////
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

        private async void feedbackButton_Click(object sender, RoutedEventArgs e) {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        private async void reviewButton_Click(object sender, RoutedEventArgs e) {
            await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store:reviewapp?appid=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }
        private async void mailButton_Click(object sender, RoutedEventArgs e) {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("ismael.em@outlook.com"));
            emailMessage.Subject = "Feedback for CoinBase Unofficial v" + v;

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            switch (((ComboBoxItem)c.SelectedItem).Name.ToString()) {
                case "EUR":
                    App.localSettings.Values["Coin"] = "EUR";
                    App.coin       = "EUR";
                    App.coinSymbol = "€";
                    break;
                case "USD":
                    App.localSettings.Values["Coin"] = "USD";
                    App.coin       = "USD";
                    App.coinSymbol = "$";
                    break;
                case "CAD":
                    App.localSettings.Values["Coin"] = "CAD";
                    App.coin       = "CAD";
                    App.coinSymbol = "€";
                    break;
                case "MXN":
                    App.localSettings.Values["Coin"] = "MXN";
                    App.coin       = "MXN";
                    App.coinSymbol = "€";
                    break;
                case "CNY":
                    App.localSettings.Values["Coin"] = "CNY";
                    App.coin       = "CNY";
                    App.coinSymbol = "¥";
                    break;
            }
        }
    }
}
