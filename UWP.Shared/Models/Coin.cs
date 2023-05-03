using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Linq;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    public partial class Coin : ObservableObject {
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
                var pinned = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.PinnedCoins);
                IsFav = pinned.Split("|").ToList().Contains(value);
            }
        }


        [ObservableProperty]
        private string fullName = "NULL";


        [ObservableProperty]
        private string logo = "/Assets/Icons/iconNULL.png";


        private bool isFav = false;
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

        [ObservableProperty]
        private string currencySym = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.CurrencySymbol);

        /// <summary>
        /// To indicate that data is loading, change the opacity of the charts
        /// and activate a Loading bar
        /// </summary>
        [ObservableProperty]
        private bool isLoading = false;


        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        [ObservableProperty]
        private SolidColorBrush diffFG = (SolidColorBrush)Application.Current.Resources["pastelGreen"];


        /// <summary>
        /// Basic data of a coin that varies over time
        /// </summary>
        [ObservableProperty]
        private double price = 0;


        private (double oldPrice, double newPrice) prices;
        public (double oldPrice, double newPrice) Prices {
            get => prices;
            set {
                prices = value;
                Diff = Math.Round((value.newPrice - value.oldPrice), 2);
                DiffPct = Math.Round(((value.newPrice - value.oldPrice) / value.oldPrice) * 100, 1);
            }
        }


        [ObservableProperty]
        private double diffPct = 0;

        [ObservableProperty]
        private double volumeToTotal = 0;

        [ObservableProperty]
        private double volumeFromTotal = 0;

        private double diff = 0;
        public double Diff {
            get => diff;
            private set {
                DiffFG = (value >= 0) ?
                    (SolidColorBrush)Application.Current.Resources["pastelGreen"] :
                    (SolidColorBrush)Application.Current.Resources["pastelRed"];
                SetProperty(ref diff, value);
            }
        }
    }
}
