using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Refit;
using System;
using UWP.Services;
using Windows.ApplicationModel.Background;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    public sealed class Tasks : XamlRenderingBackgroundTask {

        protected override async void OnRun(IBackgroundTaskInstance taskInstance) {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            /// Register services (Background task can't access services from the UWP)
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton(RestService.For<ICryptoCompare>("https://min-api.cryptocompare.com/"))
                .AddTransient<LocalSettings>()
                .BuildServiceProvider());



            // TODO: update Primary Tile
            var tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles) {
                try {
                    await LiveTileUpdater.UpdateSecondaryTile(tile.TileId);
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
