using CryptoTracker.Constants;
using CryptoTracker.Helpers;
using CryptoTracker.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Services.Store;
using Windows.System;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace CryptoTracker {
    public sealed partial class Settings : Page {

        private PackageVersion version;

        public Settings() {
            this.InitializeComponent();            

            
            version = Package.Current.Id.Version;
            VersionTextBlock.Text = "Version: " + string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);

            ThemeComboBox.PlaceholderText = App.localSettings.Values[UserSettingsConstants.UserTheme].ToString();
            FooterLogo.Source = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) ? 
                new BitmapImage(new Uri("ms-appx:///Assets/Tile-L.png")) : new BitmapImage(new Uri("ms-appx:///Assets/Tile-D.png"));
            

            switch (App.localSettings.Values[UserSettingsConstants.UserCurrency]) {
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
            CoinComboBox.PlaceholderText = App.localSettings.Values[UserSettingsConstants.UserCurrency].ToString();

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
        private async void PaypalButton_Click(object sender, RoutedEventArgs e) {
            Analytics.TrackEvent("paypalButton_Click");
            await Launcher.LaunchUriAsync(new Uri("https://paypal.me/ismaelEstalayo"));
        }

        // ###############################################################################################
        public async Task<bool> ShowRatingReviewDialog() {
            StoreContext _storeContext = StoreContext.GetDefault(); ;
            StoreRateAndReviewResult result = await _storeContext.RequestRateAndReviewAppAsync();

            switch (result.Status) {
                case StoreRateAndReviewStatus.Succeeded:
                    await LottiePlayer.PlayAsync(0, 1, false);
                    break;
                case StoreRateAndReviewStatus.CanceledByUser:
                    await LottiePlayer.PlayAsync(0, 1, false);
                    break;
                case StoreRateAndReviewStatus.Error:
                case StoreRateAndReviewStatus.NetworkError:
                    await new MessageDialog("Something went wrong.").ShowAsync();
                    break;
                default:
                    break;
            }

            // There was an error with the request, or the customer chose not to
            // rate or review the app.
            return false;
        }

        // ###############################################################################################
        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            String currency = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App.localSettings.Values[UserSettingsConstants.UserCurrency] = currency;
            App.currency = currency;
            App.currencySymbol = CurrencyHelper.CurrencyToSymbol(currency);
        }

        private async void UploadPortfolio_Click(object sender, RoutedEventArgs e) {            
            try {
                var helper = new RoamingObjectStorageHelper();
                var portfolio = await LocalStorageHelper.ReadObject<ObservableCollection<PurchaseModel>>(UserSettingsConstants.UserPortfolio);
                
                if (portfolio == null || portfolio.Count == 0) {
                    await new ContentDialog() {
                        Title = "Empty portfolio",
                        Content = "Your current portfolio is empty. Add a purchase before uploading it to the cloud.",
                        DefaultButton = ContentDialogButton.Primary,
                        PrimaryButtonText = "Export",
                        IsPrimaryButtonEnabled = false,
                        CloseButtonText = "Cancel",
                        RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
                    }.ShowAsync();
                }

                else {
                    var response = await new ContentDialog() {
                        Title = $"Export {portfolio.Count} purchases?",
                        Content = "This will create a backup of your current portfolio in the cloud.",
                        DefaultButton = ContentDialogButton.Primary,
                        PrimaryButtonText = "Export",
                        CloseButtonText = "Cancel",
                        RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
                    }.ShowAsync();

                    if (response == ContentDialogResult.Primary)
                        await helper.SaveFileAsync(UserSettingsConstants.UserPortfolio, portfolio);
                }
            }
            catch (Exception ex) {
                await new MessageDialog("Error uploading your portfolio. Try again later.").ShowAsync();
                await new ContentDialog() {
                    Title = "Error uploading your portfolio.",
                    Content = $"Please, try again later. Error: {ex}",
                    DefaultButton = ContentDialogButton.Close,
                    PrimaryButtonText = "Export",
                    IsPrimaryButtonEnabled = false,
                    CloseButtonText = "Cancel",
                    RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
                }.ShowAsync();
            }
        }

        private async void DownloadPortfolio_Click(object sender, RoutedEventArgs e) {
            var helper = new RoamingObjectStorageHelper();
            
            if (await helper.FileExistsAsync(UserSettingsConstants.UserPortfolio)) {
                var obj = await helper.ReadFileAsync<ObservableCollection<PurchaseModel>>(UserSettingsConstants.UserPortfolio);

                var response = await new ContentDialog() {
                    Title = $"Import {obj.Count} purchases?",
                    Content = "This will clear your current portfolio and download your backup.",
                    DefaultButton = ContentDialogButton.Primary,
                    PrimaryButtonText = "Import",
                    CloseButtonText = "Cancel",
                    RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
                }.ShowAsync();

                if (response == ContentDialogResult.Primary) {
                    LocalStorageHelper.SaveObject(obj, UserSettingsConstants.UserPortfolio);
                    vm.InAppNotification("Informational", "", "Portfolio imported succesfully.");
                }
            }
            else {
                await new ContentDialog() {
                    Title = "No backup found.",
                    Content = "You don't seem to have uploaded any portfolio before.",
                    DefaultButton = ContentDialogButton.Primary,
                    IsPrimaryButtonEnabled = false,
                    PrimaryButtonText = "Import",
                    CloseButtonText = "Cancel",
                    RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
                }.ShowAsync();
            }
        }

        private void ThemeComboBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            var theme = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App.localSettings.Values[UserSettingsConstants.UserTheme] = theme;
            switch (theme) {
                case "Light":
                    FooterLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Tile-D.png"));
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    FooterLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Tile-L.png"));
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;
                case "Windows":
                    if (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) {
                        ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                        FooterLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Tile-L.png"));
                    }
                    else {
                        ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                        FooterLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Tile-D.png"));
                    }
                    break;
            }
        }
        
    }
}
