using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using System;
using System.Globalization;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace UWP.Background {
    class LiveTileGenerator {
        public static XmlDocument SecondaryTile(string crypto, string currencySymbol, double price,
            Tuple<string, string> diff1d, Tuple<string, string> diff7d) {
            NumberFormatInfo nfi = new CultureInfo(CultureInfo.CurrentCulture.LCID).NumberFormat;
            return new TileContent() {
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
                                }, } }
                    },
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
                                    Text = price.ToString("N") + currencySymbol,
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveGroup() {
                                    Children = {
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = $"{diff1d.Item1}1d",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText() {
                                                    Text = diff1d.Item2,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                } }
                                        },
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = $"{diff7d.Item1}1w",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Right
                                                },
                                                new AdaptiveText() {
                                                    Text = diff7d.Item2,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Right
                                                }
                                            } }, } } } }
                    },
                    TileWide = new TileBinding() {
                        Branding = TileBranding.None,
                        Content = new TileBindingContentAdaptive() {
                            BackgroundImage = new TileBackgroundImage() {
                                Source = $"{ApplicationData.Current.LocalFolder.Path}/tile-{crypto}.png",
                                HintCrop = TileBackgroundImageCrop.None,
                                HintOverlay = 0
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
                                                    Text = $"{diff1d.Item1}1d: {diff1d.Item2}",
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                                new AdaptiveText() {
                                                    Text = DateTime.Now.ToShortTimeString(),
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                            } },
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = price.ToString("N") + currencySymbol,
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText() {
                                                    Text = $"{diff7d.Item1}1w: {diff7d.Item2}",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                            } } } } }
                        } } }
            }.GetXml();
        }
    }
}
