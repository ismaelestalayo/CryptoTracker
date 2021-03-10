using System;
using Windows.ApplicationModel.Background;
using Windows.UI.StartScreen;

namespace UWP.Background {
    public sealed class Tasks : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // TODO: update Primary Tile
            var tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles) {
                try {
                    await LiveTileUpdater.UpdateSecondaryTile(tile.TileId, null);
                }
                catch (Exception ex) {
                    var z = ex.Message;
                }
            }

            // TODO: check price alerts

            deferral.Complete();
        }
    }
}
