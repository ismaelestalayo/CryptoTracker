using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Windows.ApplicationModel.Email;
using Windows.System;
using CryptoTracker.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Threading.Tasks;
using Windows.Services.Store;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel;
using Windows.UI.Popups;

using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.ViewManagement;
using Windows.UI;

namespace CryptoTracker {
    public sealed partial class Settings : Page {

        private Package package;
        private PackageVersion version;
        private String portfolioKey = "portfolio";

        public Settings() {
            this.InitializeComponent();            

            package = Package.Current;
            version = package.Id.Version;
            VersionTextBlock.Text = "Version: " + string.Format("{0}.{1}.{2}.", version.Major, version.Minor, version.Revision);

            ThemeComboBox.PlaceholderText = App.localSettings.Values["Theme"].ToString();
            //ThemeSwitcher.IsOn = App.localSettings.Values["Theme"].Equals("Dark");


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

            // Show feedback button
            if (Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.IsSupported()) {
                this.feedbackButton.IsEnabled = true;
            } else {
                this.feedbackButton.IsEnabled = false;
            }

        }

        // ###############################################################################################
        private async void FeedbackButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("feedbackButton_Click");
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }
        private async void RatingButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("ratingButton_Click");
            await ShowRatingReviewDialog();
        }
        private async void ReviewButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("reviewButton_Click");
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9n3b47hbvblc"));
        }
        private async void MailButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("mailButton_Click");
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("ismael.em@outlook.com"));
            emailMessage.Subject = "Feedback for CryptoTracker v" + string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Revision);
            emailMessage.Body = "<Insert here suggestions/bugs/feature requests...> \n\n";

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }
        private async void TwitterButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("twitterButton_Click");
            await Launcher.LaunchUriAsync(new Uri("https://twitter.com/ismaelestalayo"));
        }
        private async void RedditButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("redditButton_Click");
            await Launcher.LaunchUriAsync(new Uri("https://www.reddit.com/r/CryptoTracker/"));
        }

        // ###############################################################################################
        public async Task<bool> ShowRatingReviewDialog() {
            StoreSendRequestResult result = await StoreRequestHelper.SendRequestAsync(
                StoreContext.GetDefault(), 16, String.Empty);

            if (result.ExtendedError == null) {
                JObject jsonObject = JObject.Parse(result.Response);
                if (jsonObject.SelectToken("status").ToString() == "success") {
                    // The customer rated or reviewed the app.
                    return true;
                }
            }

            // There was an error with the request, or the customer chose not to
            // rate or review the app.
            return false;
        }

        // ###############################################################################################
        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            String currency = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App.localSettings.Values["Coin"] = currency;
            App.coin = currency;
            App.coinSymbol = CurrencyHelper.CurrencyToSymbol(currency);
        }

        private async void UploadConfigButton_Click(object sender, RoutedEventArgs e) {
            try {
                var helper = new RoamingObjectStorageHelper();
                var portfolio = Portfolio.dataList;

                ContentDialog exportDialog = new ContentDialog() {
                    Title = $"Export {portfolio.Count} purchases?",
                    Content = "This will create a backup of your current portfolio in the cloud.",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Export",
                    CloseButtonText = "Cancel"
                };

                var response = await exportDialog.ShowAsync();                

                if (response == ContentDialogResult.Primary)
                    await helper.SaveFileAsync(portfolioKey, portfolio);
            } catch  {
                await new MessageDialog("Error uploading your portfolio. Try again later.").ShowAsync();
            }
        }

        private async void DownloadConfigButton_Click(object sender, RoutedEventArgs e) {
            var helper = new RoamingObjectStorageHelper();

            // Read complex/large objects 
            if (await helper.FileExistsAsync(portfolioKey)) {
                var obj = await helper.ReadFileAsync<List<PurchaseClass>>(portfolioKey);

                ContentDialog importDialog = new ContentDialog() {
                    Title = $"Import {obj.Count} purchases?",
                    Content = "This will clear your current portfolio and download your backup.",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Import",
                    CloseButtonText = "Cancel"
                };

                var response = await importDialog.ShowAsync();

                if (response == ContentDialogResult.Primary)
                    Portfolio.importPortfolio(obj);
            }
            else {
                ContentDialog importDialog = new ContentDialog() {
                    Title = "No backup found.",
                    Content = "You don't seem to have uploaded any portfolio before.",
                    DefaultButton = ContentDialogButton.Primary,
                    IsPrimaryButtonEnabled = false,
                    PrimaryButtonText = "Import",
                    CloseButtonText = "Cancel"
                };
                await importDialog.ShowAsync();
            }
        }

        private void Theme_RadioButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "Dark":
                    App.localSettings.Values["Theme"] = "Dark";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    Analytics.TrackEvent("theme_dark_side");
                    break;

                case "Light":
                    App.localSettings.Values["Theme"] = "Light";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    Analytics.TrackEvent("theme_light");
                    break;

                case "Windows":
                    App.localSettings.Values["Theme"] = "Windows";
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    Analytics.TrackEvent("theme_windows");
                    break;
            }
        }

        private void ThemeComboBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            var theme = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App.localSettings.Values["Theme"] = theme;
            switch (theme) {
                case "Light":
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;
                case "Windows":
                    ((Frame)Window.Current.Content).RequestedTheme = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) ? ElementTheme.Dark : ElementTheme.Light;
                    break;
            }
        }
    }
}
