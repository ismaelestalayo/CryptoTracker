using CryptoTracker.APIs;
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

namespace CryptoTracker {
    class LiveTileHelper {

        public static async Task AddSecondaryTile(string crypto, UIElement chart) {

            try {
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

                await PinSecondaryTile(crypto);

            }
            catch (Exception ex) {
                var z = ex.Message;
            }
        }

        internal static void PinPrimaryTile(string crypto) {
            //var dateUpdated = DateTime.Now;
            XmlDocument content = GeneratePortfolioTile();
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private static XmlDocument GeneratePortfolioTile() {
            var content = new TileContent() {
                Visual = new TileVisual() {
                    TileMedium = new TileBinding() {
                        Branding = TileBranding.Logo,
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText() {
                                    Text = "AAAAA",
                                    HintStyle = AdaptiveTextStyle.Body
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            BackgroundImage = new TileBackgroundImage() {
                                Source = $"{ApplicationData.Current.LocalFolder.Path}/testImage.png",
                                HintCrop = TileBackgroundImageCrop.None
                            },
                            Children = {
                                new AdaptiveGroup() {
                                    Children = {
                                        new AdaptiveSubgroup() {
                                            HintWeight = 1,
                                            HintTextStacking = AdaptiveSubgroupTextStacking.Center,
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = "20%",
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.BaseSubtle
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup() {
                                            HintWeight = 1,
                                            HintTextStacking = AdaptiveSubgroupTextStacking.Bottom,
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = "20%",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.BaseSubtle
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return content.GetXml();
        }


        /// <summary>
        /// Pin a secondary tile for a specific coin
        /// </summary>
        private static async Task PinSecondaryTile(string crypto) {
            XmlDocument content = await GenerateCoinTile(crypto);
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(crypto).Update(notification);
        }

        /// Generate the XML
        private static async Task<XmlDocument> GenerateCoinTile(string crypto) {
            var raw = await CryptoCompare.GetCoinStats(crypto);
            var price = raw.PRICE;
            var diff24 = raw.CHANGEPCT24HOUR;
            var diff11 = raw.CHANGEPCTHOUR;
            var arrow24 = diff24 < 0 ? "▼" : "▲";
            var arrow11 = diff11 < 0 ? "▼" : "▲";
            string percent24h = $"{Math.Abs(diff24):N}%";
            string percent1h = $"{Math.Abs(diff11):N}%";

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

            bool isPinned = await tile.RequestCreateAsync();
            //if (!isPinned)
            //    return;

            NumberFormatInfo nfi = new CultureInfo(CultureInfo.CurrentCulture.LCID).NumberFormat;
            nfi.NumberGroupSeparator = "";
            var content = new TileContent() {
                Visual = new TileVisual() {
                    Branding = TileBranding.Logo,
                    TileSmall = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText() {
                                    Text = crypto,
                                    HintStyle = AdaptiveTextStyle.Caption,
                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveText() {
                                    Text = price.ToString("G5", nfi),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintAlign = AdaptiveTextAlign.Right
                                }, } } },
                    TileMedium = new TileBinding() {
                        Branding = TileBranding.None,
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText() {
                                    Text = crypto,
                                    HintStyle = AdaptiveTextStyle.Base,
                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveText() {
                                    Text = price.ToString("N") + App.currencySymbol,
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveGroup() {
                                    Children = {
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = $"{arrow11}1h",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText() {
                                                    Text = percent1h,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                } }
                                        },
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = $"{arrow24}24h",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Right
                                                },
                                                new AdaptiveText() {
                                                    Text = percent24h,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Right
                                                } } }, } } } } },
                    TileWide = new TileBinding() {
                        Branding = TileBranding.None,
                        Content = new TileBindingContentAdaptive() {
                            BackgroundImage = new TileBackgroundImage() {
                                Source = $"{ApplicationData.Current.LocalFolder.Path}/tile-{crypto}.png",
                                HintCrop = TileBackgroundImageCrop.None
                            },
                            TextStacking = TileTextStacking.Top,
                            Children = {
                                new AdaptiveGroup() {
                                    Children = {
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = crypto,
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText() {
                                                    Text = $"{arrow11}1h: {percent1h}",
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                }, } },
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = price.ToString("N") + App.currencySymbol,
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText() {
                                                    Text = $"{arrow24}24h: {percent24h}",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                }, } } } } } }
                    }
                }
            };
            return content.GetXml();
        }

    }
}