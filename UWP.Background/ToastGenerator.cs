using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;

namespace UWP.Background {
    public sealed class ToastGenerator {
        /// <summary>
        /// Send a toast notification
        /// </summary>
        /// <param name="header"></param>
        /// <param name="subheader"></param>
        /// <param name="coin">Send "" to NOT redirect to a coin</param>
        public static void SendAlert(string header, string subheader, string coin) {
            var toastContent = new ToastContent() {
                Visual = new ToastVisual() {
                    BindingGeneric = new ToastBindingGeneric() {
                        Children = {
                            new AdaptiveText() {
                                Text = header,
                                HintStyle = AdaptiveTextStyle.Header,
                                HintMaxLines = 1
                            },
                            new AdaptiveText() {
                                Text = subheader,
                                HintStyle = AdaptiveTextStyle.BodySubtle
                            }
                            //new AdaptiveText() {
                            //    Text = DateTime.Now.ToString("g")
                            //}
                        }
                        //AppLogoOverride = new ToastGenericAppLogo() {
                        //    Source = $"ms-appx:///Assets/Icons/icon{coin}.png",
                        //    HintCrop = ToastGenericAppLogoCrop.Circle,
                        //    AlternateText = coin
                        //}
                    }
                },
                Launch = (coin != "") ? "/coin-" + coin : null
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
    }
}
