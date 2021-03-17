using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Services;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
	public class CoinCard : ObservableObject {
        /// <summary>
        /// Basic data of a coin that stays invariable through time
        /// </summary>
        private string _crypto = "NULL";
        internal string Crypto {
            get => _crypto;
            set {
                SetProperty(ref _crypto, value);
                // TODO
                //IconSrc = IconsHelper.GetIcon(value);
            }
        }
        internal string CryptoFullName { get; set; } = "NULL";
        internal string CryptoSymbol { get; set; } = "NULL";
        internal string Currency { get; set; } = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettingsConstants.Currency);
        internal string IconSrc { get; set; } = "/Assets/Icons/iconNULL.png";

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        private bool _isLoading = false;
        private double _opacity = 1;

        internal bool IsLoading {
            get => _isLoading;
            set {
                SetProperty(ref _isLoading, value);
                Opacity = value ? 0.33 : 1;
            }
        }

        internal double Opacity {
            get => _opacity;
            set => SetProperty(ref _opacity, value);
        }

        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        private List<ChartPoint> _chartData = new List<ChartPoint>() { new ChartPoint() { Value = 5, Date = DateTime.Today } };
        internal List<ChartPoint> ChartData {
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
        private SolidColorBrush _chartStroke = (SolidColorBrush)Application.Current.Resources["main_gray"];
        private SolidColorBrush _diffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
        private Color _chartFill1 = Color.FromArgb(64, 128, 128, 128);
        private Color _chartFill2 = Color.FromArgb(16, 128, 128, 128);
        
        internal SolidColorBrush ChartStroke {
            get => _chartStroke;
            set {
                SetProperty(ref _chartStroke, value);
                var color = value.Color;
                ChartFill1 = Color.FromArgb(64, color.R, color.G, color.B);
                ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
            }
        }
        internal SolidColorBrush DiffFG {
            get => _diffFG;
            set => SetProperty(ref _diffFG, value);
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
        private double _diff = 0;
        private string _diffArrow = "▲";
        private string _volume24 = "0";
        private string _volume24to = "0";

        internal double Price {
            get => _price;
            set => SetProperty(ref _price, value);
        }
        internal double Diff {
            get => _diff;
            set {
                SetProperty(ref _diff, Math.Abs(value)); /// Save the absolute value
                if (value < 0) {
                    DiffArrow = "▼";
                    DiffFG = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                }
                else {
                    DiffArrow = "▲";
                    DiffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                }
            }
        }
        internal string DiffArrow {
            get => _diffArrow;
            set => SetProperty(ref _diffArrow, value);
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
