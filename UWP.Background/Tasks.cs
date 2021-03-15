using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    public sealed class Tasks : XamlRenderingBackgroundTask {

        protected override async void OnRun(IBackgroundTaskInstance taskInstance) {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();


            Grid mainGrid = new Grid();
            TextBlock tText = new TextBlock() {
                HorizontalAlignment = HorizontalAlignment.Left,
                TextWrapping = TextWrapping.Wrap,
                Text = "Some Example Text",
                VerticalAlignment = VerticalAlignment.Top,
                FontSize = 72,
            };
            try {
                mainGrid.Children.Add(tText);
                RenderTargetBitmap rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(mainGrid);

                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("CurrentImage" + ".png", CreationCollisionOption.ReplaceExisting);
                using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                                         BitmapAlphaMode.Premultiplied,
                                         (uint)rtb.PixelWidth,
                                         (uint)rtb.PixelHeight,
                                         displayInformation.RawDpiX,
                                         displayInformation.RawDpiY,
                                         pixels);
                    await encoder.FlushAsync();
                }
            }
            catch (Exception ex) {
                var z = ex.Message;
            }



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
