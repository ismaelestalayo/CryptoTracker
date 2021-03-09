using System;
using Windows.ApplicationModel.Background;

namespace UWP.Background {
    public sealed class Tasks : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try {
                await LiveTileUpdater.AddSecondaryTile("ETH", null);

                //TileNotification notification = new TileNotification(template) { Tag = "ETH" };
                //TileUpdateManager.CreateTileUpdaterForSecondaryTile("ETH").Update(notification);
            }
            catch (Exception ex) {
                var z = ex.Message;
            }

            deferral.Complete();
        }
    }
}
