using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UWP.Shared.Constants;
using UWP.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {

    public sealed partial class PortfolioAnalytics : ContentDialog {
        public PortfolioAnalytics() {
            InitializeComponent();
            
        }

        public PortfolioAnalytics(PortfolioViewModel vm) {
            InitializeComponent();
            DataContext = vm;
            this.vm = vm;
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e) {

            var coins = new[] { "BTC", "ETH", "XXX", "LTC", "XRP", "ADA" };
            var vals = new[] { 5, 10, 10, 15, 20, 40 };

            double nextVal = 100;


            var seriess = new List<PieSeries<double>>();
            foreach (var purchase in vm.Portfolio) {
                seriess.Add(
                    new PieSeries<double> {
                        Values = new double[] { purchase.Worth },
                        Name = purchase.Crypto,
                        Fill = new SolidColorPaint(SKColor.Parse(
                            ColorConverterExtensions.ToHexString(
                                ColorConstants.GetCoinBrush(purchase.Crypto).Color)
                            )
                        ) { StrokeThickness = 8 }
                    }
                );
            }
            DiversificationPieChart.Series = seriess;
        }

        private void Ellipse_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            var ellipse = (sender as Windows.UI.Xaml.Shapes.Ellipse);
            ellipse.Scale = new Vector3((float)1.2);
        }

        private void Ellipse_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            var ellipse = (sender as Windows.UI.Xaml.Shapes.Ellipse);
            ellipse.Scale = new Vector3((float)1);
        }

    }
    public static class ColorConverterExtensions {
        public static string ToHexString(this Windows.UI.Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
    }
}
