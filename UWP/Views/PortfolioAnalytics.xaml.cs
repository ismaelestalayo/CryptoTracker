using System;
using System.Collections.Generic;
using UWP.Shared.Constants;
using UWP.ViewModels;
using Windows.UI.Xaml.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

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


            var seriess = new List<PieSeries<double>>();
            foreach (var purchase in vm.Portfolio) {
                seriess.Add(
                    new PieSeries<double> {
                        Values = new double[] { purchase.Worth },
                        Name = purchase.Crypto + "€"
                    }
                );
            }

            DiversificationPieChart.Series = seriess;
        }
    }
}
