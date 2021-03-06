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

namespace CryptoTracker {
    class LiveTile {

        public static async void UpdateLiveTile(UIElement chart) {

            try {
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(chart);
                var pixelBuffer = await rtb.GetPixelsAsync();
                var pixels = pixelBuffer.ToArray();
                var displayInformation = DisplayInformation.GetForCurrentView();

                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("testImage.png", CreationCollisionOption.ReplaceExisting);
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

                SendStockTileNotification("ETH", 1462, 5.65, DateTime.Now);

            }
            catch (Exception ex) {
                var z = ex.Message;
            }
        }

        internal static void SendStockTileNotification(string crypto, double price, double diff, DateTime dateUpdated) {
            XmlDocument content = GenerateCoinTile(crypto, price, diff, dateUpdated);
            TileNotification notification = new TileNotification(content);
            notification.Tag = crypto;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private static XmlDocument GenerateCoinTile(string crypto, double price, double diff, DateTime dateUpdated) {
            var arrow = diff < 0 ? "▼" : "▲";
            diff = Math.Abs(diff);
            string percentString = $"{arrow} {diff:N}%";

            var content = new TileContent() {
                Visual = new TileVisual() {
                    TileMedium = new TileBinding() {
                        Arguments = crypto,
                        Branding = TileBranding.Logo,
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText() {
                                    Text = crypto,
                                    HintStyle = AdaptiveTextStyle.Body
                                },
                                new AdaptiveText() {
                                    Text = price.ToString("N") + App.currencySymbol,
                                    HintStyle = AdaptiveTextStyle.Body
                                },
                                new AdaptiveText() {
                                    Text = $"1d: {percentString}",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },
                                new AdaptiveText() {
                                    Text = $"1w: {percentString}",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
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

        private static AdaptiveSubgroup CreateSubgroup(string crypto, string price, string diff) =>
            new AdaptiveSubgroup() {
                HintWeight = 1,
                Children = {
                    new AdaptiveText() {
                        Text = crypto,
                        HintAlign = AdaptiveTextAlign.Center
                    },
                    new AdaptiveText() {
                        Text = diff
                    },
                    new AdaptiveText() {
                        Text = diff,
                        HintAlign = AdaptiveTextAlign.Center
                    }
                }
            };
    }
}