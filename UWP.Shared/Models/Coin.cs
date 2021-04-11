using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Linq;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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
                Logo = IconsHelper.GetIcon(value);
                var coinList = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.PinnedCoins);
                IsFav = coinList.Split("|").ToList().Contains(value);
            }
        }
        public string FullName { get; set; } = "NULL";
        public string Logo { get; set; } = "/Assets/Icons/iconNULL.png";


        private bool isFav  = false;
        private bool isPin = false;
        public bool IsFav {
            get => isFav;
            set {
                if (SetProperty(ref isFav, value))
                    OnPropertyChanged("FavIcon");
            }
        }
        public bool IsPin {
            get => isPin;
            set {
                if (SetProperty(ref isPin, value))
                    OnPropertyChanged("PinIcon");
            }
        }

        public string FavIcon => IsFav ? "\xEB52" : "\xEB51";
        public string PinIcon => IsPin ? "\xE77A" : "\xE840";

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
        private double volumeToTotal = 0;
        private double volumeFromTotal = 0;

        public double Price {
            get => price;
            set => SetProperty(ref price, value);
        }
        public double Diff {
            get => diff;
            set {
                SetProperty(ref diff, value);
                DiffFG = (value > 0) ?
                    (SolidColorBrush)Application.Current.Resources["pastelGreen"] :
                    (SolidColorBrush)Application.Current.Resources["pastelRed"];
            }
        }

        public double VolumeToTotal {
            get => volumeToTotal;
            set => SetProperty(ref volumeToTotal, value);
        }
        public double VolumeFromTotal {
            get => volumeFromTotal;
            set => SetProperty(ref volumeFromTotal, value);
        }
    }
}
