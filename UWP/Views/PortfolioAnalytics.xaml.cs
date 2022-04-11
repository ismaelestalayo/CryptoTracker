using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using UWP.Shared.Constants;
using UWP.ViewModels;
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

        private void ContentDialog_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e) {

            var coins = new[] { "BTC", "ETH", "XXX", "LTC", "XRP", "ADA" };
            var vals = new[] { 5, 10, 10, 15, 20, 40 };

            double nextVal = 100;
            var entries = new List<ChartEntry>();

            

            for (int i = 0; i < coins.Length; i++) {
                entries.Add(new ChartEntry(vals[i]) {
                    Color = SKColor.Parse(ColorConstants.GetColorBrush("coin_" + coins[i]).Color.ToString()),
                    Label = coins[i],
                    ValueLabel = vals[i].ToString() + "%",
                    ValueLabelColor = SKColors.Gray
                });
            }

            chartView.Chart = new DonutChart() {
                BackgroundColor = SKColors.Transparent,
                Entries = entries,
                LabelColor = SKColors.Green,
                AnimationDuration = TimeSpan.FromSeconds(1),
                LabelMode = LabelMode.LeftAndRight
            };

            //foreach (var purchase in vm.Portfolio) {

            //}
        }
    }
}
