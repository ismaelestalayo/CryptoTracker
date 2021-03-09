using System;
using Windows.ApplicationModel.Background;
using Windows.UI.StartScreen;

namespace UWP.Background {
    public sealed class Tasks : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try {
                var tiles = await SecondaryTile.FindAllAsync();
                foreach (var tile in tiles)
                    await LiveTileUpdater.AddSecondaryTile(tile.TileId, null);

            }
            catch (Exception ex) {
                var z = ex.Message;
            }

            deferral.Complete();
        }
    }
}
