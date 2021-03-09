using System;
using System.Text;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace UWP.Background {
    public sealed class Tasks : IBackgroundTask {
        public void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try {
                var template = new StringBuilder();
                template.Append("<tile>");
                template.Append("<visual branding='name'>");
                template.Append("<binding template='TileMedium'>");
                template.Append("<text hint-style='caption'>9:50 AM, Wednesday</text>");
                template.Append("<text hint-style='captionSubtle' hint-wrap='true'>263 Grove St, San Francisco, CA 94102</text>");
                template.Append("</binding>");
                template.Append("</visual>");
                template.Append("</tile>");

                //TileNotification notification = new TileNotification(template) { Tag = "ETH" };
                //TileUpdateManager.CreateTileUpdaterForSecondaryTile("ETH").Update(notification);

                var xml = new XmlDocument();
                xml.LoadXml(template.ToString());
                ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(xml));
            }
            catch (Exception ex) {
                var z = ex.Message;
            }

            deferral.Complete();
        }
    }
}
