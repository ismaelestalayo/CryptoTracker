//using CryptoTracker.APIs;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using System;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    class LiveTileUpdater {

        public static async Task AddSecondaryTile(string crypto, UIElement chart = null) {

            if (chart != null) {
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(chart);
                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"tile-{crypto}.png", CreationCollisionOption.ReplaceExisting);
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

            XmlDocument content = await GenerateCoinTile(crypto);
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(crypto).Update(notification);
        }


        /// Generate the XML
        private static async Task<XmlDocument> GenerateCoinTile(string crypto) {
            //var raw = await CryptoCompare.GetCoinStats(crypto);
            var price = 1533.18; // raw.PRICE;
            var diff1 = 4.3; // raw.CHANGEPCT24HOUR;
            var diff2 = 0.28; // raw.CHANGEPCTHOUR;

            var arrow1d = diff1 < 0 ? "▼" : "▲";
            var arrow1h = diff2 < 0 ? "▼" : "▲";
            var diff1h = new Tuple<string, string>(arrow1h, $"{Math.Abs(diff1):N}%");
            var diff1d = new Tuple<string, string>(arrow1d, $"{Math.Abs(diff2):N}%");
            string currencySymbol = "€";

            // Initialize the tile with required arguments
            SecondaryTile tile = new SecondaryTile(
                crypto,
                "CryptoTracker",
                $"/tile-{crypto}",
                new Uri("ms-appx:///Assets/Tiles and stuff/Tile-Medium.scale-100.png"),
                TileSize.Wide310x150);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnSquare310x310Logo = false;
            tile.VisualElements.ShowNameOnWide310x150Logo = false;
            tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Tiles and stuff/Tile-Wide.scale-100.png");

            if (!SecondaryTile.Exists(crypto))
                await tile.RequestCreateAsync();

            return LiveTileGenerator.SecondaryTile(crypto, currencySymbol, price, diff1h, diff1d);
        }

    }
}