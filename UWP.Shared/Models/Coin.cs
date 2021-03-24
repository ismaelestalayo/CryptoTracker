using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using UWP.Services;
using UWP.Core.Constants;
using System.Linq;

namespace UWP.Models {
	public class Coin : ObservableObject {
        /// <summary>
        /// Basic data of a coin that stays invariable through time
        /// </summary>
        private string name = "NULL";
        public string Name {
            get => name;
            set {
                SetProperty(ref name, value);
                // TODO
                //Logo = IconsHelper.GetIcon(value);
                var coinList = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.PinnedCoins);
                IsFav = coinList.Split("|").ToList().Contains(value);
            }
        }
        public string FullName { get; set; } = "NULL";
        public string Logo { get; set; } = "/Assets/Icons/iconNULL.png";
        public bool IsFav { get; set; } = false;

        private string currencySym = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.CurrencySymbol);
        public string CurrencySym {
            get => currencySym;
            set => SetProperty(ref currencySym, value);
        }

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        private bool isLoading = false;
        private double opacity = 1;

        public bool IsLoading {
            get => isLoading;
            set {
                SetProperty(ref isLoading, value);
                Opacity = value ? 0.33 : 1;
            }
        }

        public double Opacity {
            get => opacity;
            set => SetProperty(ref opacity, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush diffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
        public SolidColorBrush DiffFG {
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

        public double Price {
            get => price;
            set => SetProperty(ref price, value);
        }
        public double Diff {
            get => diff;
            set {
                SetProperty(ref diff, Math.Abs(value)); /// Save the absolute value
                if (value > 0) {
                    DiffArrow = "▲";
                    DiffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                }
                else {
                    DiffArrow = "▼";
                    DiffFG = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                }
            }
        }
        public string DiffArrow {
            get => diffArrow;
            set => SetProperty(ref diffArrow, value);
        }
        public string Volume {
            get => volume;
            set => SetProperty(ref volume, value);
        }
    }
}
