using System;
using System.Collections.Generic;
using System.Linq;
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
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace UWP.Background {
    public sealed class LiveTileUpdater {

        internal static string currency = (string)ApplicationData.Current.LocalSettings.Values["Currency"];
        internal static string currencySymbol = "X";
        internal static List<HistoricPrice> hist;

        /// <summary>
        /// Cant have Public methods returning Task in a Windows Runtime Component:
        /// 
        /// https://marcominerva.wordpress.com/2013/03/21/how-to-expose-async-methods-in-a-windows-runtime-component/
        /// </summary>
        public static IAsyncAction AddSecondaryTile(string crypto, UIElement chart) {
            return UpdateSecondaryTile(crypto, chart).AsAsyncAction();
        }

        internal static async Task UpdateSecondaryTile(string crypto, UIElement chart = null) {
            hist = await GetHistoDupe.GetWeeklyHistAsync(crypto);
            await RenderTileSVG();

            XmlDocument content = await GenerateCoinTile(crypto);
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(crypto).Update(notification);
        }


        /// Generate the XML
        private static async Task<XmlDocument> GenerateCoinTile(string crypto) {
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

        private static async Task RenderTileSVG() {
            var polyline = new Polyline();
            polyline.Stroke = new SolidColorBrush(Windows.UI.Colors.Red);
            polyline.StrokeThickness = 2;

            var points = new PointCollection();
            int i = 0;
            var ordered = hist.OrderByDescending(x => x.Average);
            double min = ordered.LastOrDefault().Average;
            double max = ordered.FirstOrDefault().Average;
            foreach (var h in hist)
                points.Add(new Point(2*i++, 150 - (150 * ((h.Average - min) / (max - min)))));
            polyline.Points = points;

            //var grid = new Grid();
            //grid.Children.Add(polyline);
            //polyline.Height = 150;
            //polyline.Width = 300;

            try {
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(polyline);
                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync($"testSVG.png",
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
            catch (Exception ex) {
                var z = ex.Message;
            }
        }
    }
}