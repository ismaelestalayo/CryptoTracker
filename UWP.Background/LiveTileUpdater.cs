using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    public sealed class LiveTileUpdater {

        internal static string currency = (string)ApplicationData.Current.LocalSettings.Values["Currency"];
        internal static string currencySymbol = "X";

        /// <summary>
        /// Cant have Public methods returning Task in a Windows Runtime Component:
        /// </summary>
        public static IAsyncAction AddSecondaryTile(string crypto, UIElement chart) {
            return UpdateSecondaryTile(crypto, chart).AsAsyncAction();
        }

        internal static async Task UpdateSecondaryTile(string crypto, UIElement chart = null) {

            if (chart != null) {
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(chart);
                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"tile-{crypto}.png",
                    CreationCollisionOption.ReplaceExisting);
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
            var hist = await GetHistoDupe.GetWeeklyHistAsync(crypto);
            var count = hist.Count - 1;

            var price = Rounder(hist[count].Average);
            var _diff1d = ((price - hist[count - 25].Average) / price) * 100;
            var _diff7d = ((price - hist[0].Average) / price) * 100;

            var arrow1d = _diff1d < 0 ? "▼" : "▲";
            var arrow7d = _diff7d < 0 ? "▼" : "▲";
            var diff1d = new Tuple<string, string>(arrow1d, $"{Math.Abs(_diff1d):N}%");
            var diff7d = new Tuple<string, string>(arrow7d, $"{Math.Abs(_diff7d):N}%");

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

            return LiveTileGenerator.SecondaryTile(crypto, currencySymbol, price, diff1d, diff7d);
        }

        public static double Rounder(double price) {
            if (Math.Abs(price) > 99)
                return Math.Round(price, 2);
            else if (Math.Abs(price) > 10)
                return Math.Round(price, 3);
            else if (Math.Abs(price) > 1)
                return Math.Round(price, 4);
            else
                return Math.Round(price, 6);
        }
    }
}