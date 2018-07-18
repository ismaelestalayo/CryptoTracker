using Microsoft.AppCenter.Analytics;
using System;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel.Email;
using Windows.System;
using CryptoTracker.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CryptoTracker {
    public sealed partial class Settings : Page {

        private string v;

        public Settings() {
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
                case "GBP":
                    GBP.IsSelected = true;
                    break;
                case "CAD":
                    CAD.IsSelected = true;
                    break;
                case "AUD":
                    AUD.IsSelected = true;
                    break;
                case "MXN":
                    MXN.IsSelected = true;
                    break;
                case "CNY":
                    CNY.IsSelected = true;
                    break;
                case "JPY":
                    JPY.IsSelected = true;
                    break;
                case "INR":
                    INR.IsSelected = true;
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

            Analytics.TrackEvent("ThemeToogled");
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

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("feedbackButton_Click");
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        private async void ReviewButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("reviewButton_Click");
            await Launcher.LaunchUriAsync(new Uri(@"ms-windows-store:reviewapp?appid=" + Windows.ApplicationModel.Store.CurrentApp.AppId));
        }
        private async void MailButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("mailButton_Click");
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("ismael.em@outlook.com"));
            emailMessage.Subject = "Feedback for CoinBase Unofficial v" + v;

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
        private async void TwitterButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("twitterButton_Click");
            await Launcher.LaunchUriAsync(new Uri("https://twitter.com/isma_estalayo"));
        }
        private async void RedditButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("redditButton_Click");
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/CryptoTracker/"));
        }

        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            String currency = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App.localSettings.Values["Coin"] = currency;
            App.coin = currency;
            App.coinSymbol = CurrencyHelper.CurrencyToSymbol(currency);
        }
    }
}
