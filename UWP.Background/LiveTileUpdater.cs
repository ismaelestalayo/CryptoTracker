using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Services;
using UWP.Shared.Models;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    public sealed class LiveTileUpdater {
        private static LocalSettings localSettings = new LocalSettings();
        internal static string currency = localSettings.Get<string>(UserSettings.Currency);
        internal static string currencySymbol = localSettings.Get<string>(UserSettings.CurrencySymbol);
        internal static List<HistoricPrice> hist;

        /// <summary>
        /// Cant have Public methods returning Task in a Windows Runtime Component:
        /// 
        /// https://marcominerva.wordpress.com/2013/03/21/how-to-expose-async-methods-in-a-windows-runtime-component/
        /// </summary>
        public static IAsyncOperation<bool> AddSecondaryTileAction(string crypto) {
            return AddSecondaryTile(crypto).AsAsyncOperation();
        }

        public static IAsyncOperation<bool> RemoveSecondaryTileAction(string crypto) {
            return RemoveSecondaryTile(crypto).AsAsyncOperation();
        }

        /// #######################################################################################
        internal static async Task<bool> RemoveSecondaryTile(string crypto) {
            var tiles = await SecondaryTile.FindAllAsync();
            var tile = tiles.FirstOrDefault(t => t.TileId.Equals(crypto));
            
            if (tile != null)
                return await tile.RequestDeleteAsync();
            else
                return false;
        }

        internal static async Task<bool> AddSecondaryTile(string crypto) {
            hist = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric_(crypto, "hour", 168);
            await RenderTileSVG(crypto);

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
            tile.VisualElements.Square44x44Logo = new Uri("ms-appx:///Assets/Tiles and stuff/Tile-Wide.scale-100.png");

            tile.VisualElements.ForegroundText = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black)
                ? ForegroundText.Light : ForegroundText.Dark;

            if (!SecondaryTile.Exists(crypto)) {
                if (!await tile.RequestCreateAsync())
                    return false;
            }
            else
                await tile.UpdateAsync();

            XmlDocument content = await GenerateCoinTile(crypto);
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(crypto).Update(notification);
            return true;
        }


        /// Generate the XML
        private static async Task<XmlDocument> GenerateCoinTile(string crypto) {
            var count = hist.Count - 1;

            var price = Rounder(hist[count].Average);
            var _diff1d = ((price - hist[count - 25].Average) / price) * 100;
            var _diff7d = ((price - hist[0].Average) / price) * 100;

            var arrow1d = _diff1d < 0 ? "24h: ▼" : "24h: ▲";
            var arrow7d = _diff7d < 0 ? " 7d: ▼" : " 7d: ▲";
            var diff1d = new Tuple<string, string>(arrow1d, $"{Math.Abs(_diff1d):N}%");
            var diff7d = new Tuple<string, string>(arrow7d, $"{Math.Abs(_diff7d):N}%");

            return LiveTileGenerator.SecondaryTileXML(crypto, currencySymbol, price, diff1d, diff7d);
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

        private static async Task RenderTileSVG(string crypto) {
            var grid = await LiveTileGenerator.SecondaryTileGrid(crypto);

            try {
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(grid);
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
            catch (Exception ex) {
                var z = ex.Message;
            }
        }
    }
}