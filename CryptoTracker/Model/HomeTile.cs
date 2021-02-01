using CryptoTracker.Helpers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Model {
    internal class HomeTile : ObservableObject {
        /// <summary>
        /// Basic data of a coin that stays invariable through time
        /// </summary>
        internal string CryptoName { get; set; } = "NULL";
        internal string Currency { get; set; } = App.coinSymbol;
        internal string Crypto { get; set; } = "NULL";
        internal string IconSrc { get; set; } = "/Assets/Icons/iconNULL.png";

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        private bool _isLoading = false;
        private double _opacity = 1;

        internal bool IsLoading {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }        
        internal double Opacity {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        private List<ChartData> _chartData = new List<ChartData>() { new ChartData() { Value = 5, Date = DateTime.Today } };
        internal List<ChartData> ChartData {
            get => _chartData;
            set => SetProperty(ref _chartData, value);
        }

        /// <summary>
        /// Tuple with the minimum and maximum value to adjust the chart
        /// </summary>
        private (float Min, float Max) _pricesMinMax = (0, 100);
        internal (float Min, float Max) PricesMinMax {
            get => _pricesMinMax;
            set => SetProperty(ref _pricesMinMax, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush _chartStroke = (SolidColorBrush)App.Current.Resources["Main_WhiteBlack"];
        private SolidColorBrush _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
        private Color _chartFill1 = ((SolidColorBrush)App.Current.Resources["pastelGreen"]).Color;
        private Color _chartFill2 = ((SolidColorBrush)App.Current.Resources["pastelGreen"]).Color;
        

        internal SolidColorBrush ChartStroke {
            get => _chartStroke;
            set => SetProperty(ref _chartStroke, value);
        }
        internal SolidColorBrush PriceDiffFG {
            get => _priceDiffFG;
            set => SetProperty(ref _priceDiffFG, value);
        }
        internal Color ChartFill1 {
            get => _chartFill1;
            set => SetProperty(ref _chartFill1, value);
        }
        internal Color ChartFill2 {
            get => _chartFill2;
            set => SetProperty(ref _chartFill2, value);
        }


        /// <summary>
        /// Basic data of a coin that varies over time
        /// </summary>
        private double _price = 0;
        private double _priceDiff = 0;
        private string _priceDiffArrow = "▲";
        private string _volume24 = "0";
        private string _volume24to = "0";

        internal double Price {
            get => _price;
            set => SetProperty(ref _price, value);
        }
        internal double PriceDiff {
            get => _priceDiff;
            set {
                SetProperty(ref _priceDiff, value);
                if (value > 0) {
                    PriceDiffArrow = "▲";
                    PriceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
                }
                else {
                    PriceDiffArrow = "▼";
                    PriceDiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                }
            }
        }        
        internal string PriceDiffArrow {
            get => _priceDiffArrow;
            set => SetProperty(ref _priceDiffArrow, value);
        }
        internal string Volume24 {
            get => _volume24;
            set => SetProperty(ref _volume24, value);
        }        
        internal string Volume24to {
            get => _volume24to;
            set => SetProperty(ref _volume24to, value);
        }

        
    }
}
