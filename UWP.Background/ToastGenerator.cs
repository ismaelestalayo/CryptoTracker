using NotificationsExtensions;
using NotificationsExtensions.Toasts;
using System;
using Windows.UI.Notifications;

namespace UWP.Background {
    public sealed class ToastGenerator {
        public static void SendToastNotification(string header) {
            var toastContent = new ToastContent() {
                Visual = new ToastVisual() {
                    BindingGeneric = new ToastBindingGeneric() {
                        Children = {
                            new AdaptiveText() {
                                Text = header
                            },
                            new AdaptiveText() {
                                Text = DateTime.Now.ToString("g")
                            }
                        }
                    }
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
    }
}
