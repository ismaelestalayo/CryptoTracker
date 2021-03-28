using Microsoft.AppCenter.Analytics;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.Services.Store;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsFeedback : Page {

        private PackageVersion version;

        public SettingsFeedback() {
            this.InitializeComponent();

            version = Package.Current.Id.Version;
        }

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
            emailMessage.Subject = "Feedback for CryptoTracker v" + string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
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
    }
}
