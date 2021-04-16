using Microsoft.Toolkit.Mvvm.DependencyInjection;
using NotificationsExtensions;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UWP.Services;
using UWP.Shared.Constants;
using UWP.Shared.Models;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWP.Background {
    public sealed class LiveTileGenerator {

        public static IAsyncOperation<Grid> SecondaryTileGridOperation(string crypto) {
            return SecondaryTileGrid(crypto).AsAsyncOperation();
        }

        /// <summary>
        /// Generates the a Secondary Tile's background
        /// </summary>
        internal static async Task<Grid> SecondaryTileGrid(string crypto, List<HistoricPrice> hist = null) {
            if (hist == null)
                hist = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric_(crypto, "hour", 168);

            var polyline = new Polyline();
            polyline.Stroke = ColorConstants.GetBrush($"{crypto}_color");
            polyline.Fill = ColorConstants.GetBrush($"{crypto}_color", 20);
            polyline.FillRule = FillRule.Nonzero;
            polyline.StrokeThickness = 0.5;
            polyline.VerticalAlignment = VerticalAlignment.Bottom;

            var points = new PointCollection();
            int i = 0;
            var ordered = hist.OrderByDescending(x => x.Average);
            double min = ordered.LastOrDefault().Average;
            double max = ordered.FirstOrDefault().Average;
            foreach (var h in hist.GetRange(hist.Count - 150, 150))
                points.Add(new Point(2 * ++i, 90 - (90 * ((h.Average - min) / (max - min)))));
            points.Add(new Point(2 * i, 90 ));
            points.Add(new Point(0, 90));
            polyline.Points = points;
            polyline.VerticalAlignment = VerticalAlignment.Bottom;

            var grid = new Grid() {
                Background = new SolidColorBrush(Color.FromArgb(0, 128, 128, 128)),
                Width = 300, Height = 150,
            };
            grid.Children.Add(polyline);
            return grid;
        }

        /// <summary>
        /// Generates the XML content of a Secondary Tile
        /// </summary>
        internal static XmlDocument SecondaryTileXML(string crypto, string currencySymbol, double price,
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
                                    HintStyle = AdaptiveTextStyle.Base,
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
                                    Text = $"{price}{currencySymbol}",
                                    HintStyle = AdaptiveTextStyle.Body,
                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveText() {
                                    Text = $"{diff1d.Item1}{diff1d.Item2}",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintAlign = AdaptiveTextAlign.Right
                                },
                                new AdaptiveText() {
                                    Text = $"{diff7d.Item1}{diff7d.Item2}",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintAlign = AdaptiveTextAlign.Right
                                }
                            } }
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
                                                    Text = $"{diff1d.Item1}{diff1d.Item2}",
                                                    HintAlign = AdaptiveTextAlign.Left,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                            } },
                                        new AdaptiveSubgroup() {
                                            Children = {
                                                new AdaptiveText() {
                                                    Text = $"{price}{currencySymbol}",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.Base
                                                },
                                                new AdaptiveText() {
                                                    Text = $"{diff7d.Item1}{diff7d.Item2}",
                                                    HintAlign = AdaptiveTextAlign.Right,
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                                },
                                            } } } } }
                        } } }
            }.GetXml();
        }
    }
}
