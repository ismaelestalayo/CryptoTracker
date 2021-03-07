using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.StartScreen;
using System.Threading.Tasks;
using CryptoTracker.APIs;
using System.Globalization;

namespace CryptoTracker {
    class LiveTile {

        public static async void UpdateLiveTile(UIElement chart) {

            try {
                //var rtb = new RenderTargetBitmap();
                //await rtb.RenderAsync(chart);
                //var pixelBuffer = await rtb.GetPixelsAsync();
                //var pixels = pixelBuffer.ToArray();
                //var displayInformation = DisplayInformation.GetForCurrentView();

                //var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("testImage.png", CreationCollisionOption.ReplaceExisting);
                //using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite)) {
                //    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                //    encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                //                         BitmapAlphaMode.Premultiplied,
                //                         (uint)rtb.PixelWidth,
                //                         (uint)rtb.PixelHeight,
                //                         displayInformation.RawDpiX,
                //                         displayInformation.RawDpiY,
                //                         pixels);
                //    await encoder.FlushAsync();
                //}

                //SendStockTileNotification("ETH", 1462, 5.65, DateTime.Now);

                await PinSecondaryTile("ETH");

            }
            catch (Exception ex) {
                var z = ex.Message;
            }
        }

        internal static void SendStockTileNotification(string crypto, double price, double diff, DateTime dateUpdated) {
            XmlDocument content = GeneratePortfolioTile();
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private static XmlDocument GeneratePortfolioTile() {
            var content = new TileContent() {
                Visual = new TileVisual() {
                    TileMedium = new TileBinding() {
                        Arguments = "portfolio",
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

        /// ###################################################################
        private static async Task PinSecondaryTile(string crypto) {
            XmlDocument content = await GenerateCoinTile(crypto);
            TileNotification notification = new TileNotification(content) { Tag = crypto };
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(crypto).Update(notification);
        }

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
                crypto,
                new Uri("ms-appx:///Assets/Tiles and stuff/Tile-Small.scale-400_altform-colorful_theme-light.png"),
                TileSize.Wide310x150);
            tile.VisualElements.ShowNameOnSquare150x150Logo = true;
            tile.VisualElements.ShowNameOnSquare310x310Logo = true;
            tile.VisualElements.ShowNameOnWide310x150Logo = true;
            //tile.Logo = new Uri("ms-appx:///Assets/AppIcon-D.png");
            
            bool isPinned = await tile.RequestCreateAsync();
            //if (!isPinned)
            //    return;

            NumberFormatInfo nfi = new CultureInfo(CultureInfo.CurrentCulture.LCID).NumberFormat;
            nfi.NumberGroupSeparator = "";
            var content = new TileContent() {
                Visual = new TileVisual() {
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
                                },
                            }
                        }
                    },
                    TileMedium = new TileBinding() {
                        Arguments = crypto,
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
                                                }
                                            }
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
                                                }
                                            }
                                        },
                                    }
                                },
                                //new AdaptiveText() {
                                //    Text = $"24h: {percent24h}",
                                //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                //},
                                //new AdaptiveText() {
                                //    Text = $"1h: {percent1h}",
                                //    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                //}
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
                                                },
                                                new AdaptiveText() {
                                                    Text = "ETH",
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
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
                                                },
                                                new AdaptiveText() {
                                                    Text = "1520€",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
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

    }
}