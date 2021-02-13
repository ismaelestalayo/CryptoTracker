using CryptoTracker.Helpers;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Models {
	public class Coin : ObservableObject {
        /// <summary>
        /// Basic data of a coin that stays invariable through time
        /// </summary>
        private string name = "NULL";
        internal string Name {
            get => name;
            set {
                SetProperty(ref name, value);
                Logo = IconsHelper.GetIcon(value);
            }
        }
        internal string FullName { get; set; } = "NULL";
        internal string Currency { get; set; } = App.currencySymbol;
        internal string Logo { get; set; } = "/Assets/Icons/iconNULL.png";
        internal bool IsFav { get; set; } = false;

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        private bool isLoading = false;
        private double opacity = 1;

        internal bool IsLoading {
            get => isLoading;
            set {
                SetProperty(ref isLoading, value);
                Opacity = value ? 0.33 : 1;
            }
        }

        internal double Opacity {
            get => opacity;
            set => SetProperty(ref opacity, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush diffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
        internal SolidColorBrush DiffFG {
            get => diffFG;
            set => SetProperty(ref diffFG, value);
        }


        /// <summary>
        /// Basic data of a coin that varies over time
        /// </summary>
        private double price = 0;
        private double diff = 0;
        private string diffArrow = "▲";
        private string volume = "0";

        internal double Price {
            get => price;
            set => SetProperty(ref price, value);
        }
        internal double Diff {
            get => diff;
            set {
                SetProperty(ref diff, Math.Abs(value)); /// Save the absolute value
                if (value > 0) {
                    DiffArrow = "▲";
                    DiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
                }
                else {
                    DiffArrow = "▼";
                    DiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                }
            }
        }
        internal string DiffArrow {
            get => diffArrow;
            set => SetProperty(ref diffArrow, value);
        }
        internal string Volume {
            get => volume;
            set => SetProperty(ref volume, value);
        }
    }
}
