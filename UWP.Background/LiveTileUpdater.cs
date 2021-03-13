using System;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
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
            var raw = await GetCoinStats(crypto);
            var price = raw.PRICE;
            var diff1 = raw.CHANGEPCT24HOUR;
            var diff2 =  raw.CHANGEPCTHOUR;

            var arrow1d = diff1 < 0 ? "▼" : "▲";
            var arrow1h = diff2 < 0 ? "▼" : "▲";
            var diff1h = new Tuple<string, string>(arrow1h, $"{Math.Abs(diff1):N}%");
            var diff1d = new Tuple<string, string>(arrow1d, $"{Math.Abs(diff2):N}%");

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


        internal async static Task<Raw> GetCoinStats(string crypto) {
            var URL = string.Format("https://min-api.cryptocompare.com/data/pricemultifull?fsyms={0}&tsyms={1}&e=CCCAGG",
                crypto, currency);

            try {
                var client = new HttpClient();
                var responseString= await client.GetStringAsync(new Uri(URL));

                var response = JsonSerializer.Deserialize<object>(responseString);
                var data = ((JsonElement)response).GetProperty("RAW").GetProperty(crypto).GetProperty(currency);
                var raw = JsonSerializer.Deserialize<Raw>(data.ToString());

                /// quick fixes
                raw.PRICE = Rounder(raw.PRICE);
                raw.CHANGEPCT24HOUR = Rounder(raw.CHANGEPCT24HOUR);
                raw.CHANGE24HOUR = Rounder(raw.CHANGE24HOUR);
                raw.PRICE = Rounder(raw.PRICE);

                return raw;
            }
            catch (Exception ex) {
                return new Raw();
            }
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
    public sealed class Raw {
        public string TYPE { get; set; }
        public string MARKET { get; set; }
        public string FROMSYMBOL { get; set; }
        public string TOSYMBOL { get; set; }
        public double PRICE { get; set; } = 0;
        public double MEDIAN { get; set; } = 0;
        public double VOLUME24HOUR { get; set; } = 0;
        public double VOLUME24HOURTO { get; set; } = 0;

        public double OPEN24HOUR { get; set; } = 0;
        public double HIGH24HOUR { get; set; } = 0;
        public double LOW24HOUR { get; set; } = 0;

        public double VOLUMEHOUR { get; set; } = 0;
        public double VOLUMEHOURTO { get; set; } = 0;
        public double OPENHOUR { get; set; } = 0;
        public double HIGHHOUR { get; set; } = 0;
        public double LOWHOUR { get; set; } = 0;

        public double CHANGE24HOUR { get; set; } = 0;
        public double CHANGEPCT24HOUR { get; set; } = 0;
        public double CHANGEDAY { get; set; } = 0;
        public double CHANGEPCTDAY { get; set; } = 0;
        public double CHANGEHOUR { get; set; } = 0;
        public double CHANGEPCTHOUR { get; set; } = 0;

        public double SUPPLY { get; set; } = 0;
        public double MKTCAP { get; set; } = 0;
        public double TOTALVOLUME24H { get; set; } = 0;
        public double TOTALVOLUME24HTO { get; set; } = 0;
    }
}