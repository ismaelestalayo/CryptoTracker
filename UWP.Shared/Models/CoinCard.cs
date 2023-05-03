using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    public partial class CoinCard : ObservableObject {
        /// <summary>
        /// Basic data of a coin that stays invariable through time
        /// </summary>
        private string crypto = "NULL";
        internal string Crypto {
            get => crypto;
            set {
                SetProperty(ref crypto, value);
                // TODO
                //IconSrc = IconsHelper.GetIcon(value);
            }
        }
        internal string CryptoFullName { get; set; } = "NULL";
        internal string CryptoSymbol { get; set; } = "NULL";
        internal string Currency { get; set; } = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.Currency);
        internal string IconSrc { get; set; } = "/Assets/Icons/iconNULL.png";

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        [ObservableProperty]
        private bool isLoading = false;


        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        [ObservableProperty]
        private List<ChartPoint> chartData = new List<ChartPoint>() { new ChartPoint() { Value = 5, Date = DateTime.Today } };

        /// <summary>
        /// Tuple with the minimum and maximum value to adjust the chart
        /// </summary>
        [ObservableProperty]
        private (float Min, float Max) pricesMinMax = (0, 100);

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        [ObservableProperty]
        private SolidColorBrush chartStroke = (SolidColorBrush)Application.Current.Resources["maingray"];

        [ObservableProperty]
        private SolidColorBrush diffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];


        /// <summary>
        /// Basic data of a coin that varies over time
        /// </summary>
        [ObservableProperty]
        private double price = 0;

        [ObservableProperty]
        private string volume24 = "0";

        [ObservableProperty]
        private string volume24to = "0";

        private double diff = 0;
        internal double Diff {
            get => diff;
            set {
                SetProperty(ref diff, value);
                DiffFG = (value < 0) ?
                    (SolidColorBrush)Application.Current.Resources["pastelRed"] :
                    (SolidColorBrush)Application.Current.Resources["pastelGreen"];
            }
        }
    }
}
