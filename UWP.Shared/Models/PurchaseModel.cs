using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UWP.Core.Constants;
using UWP.Services;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    /// <summary>
    /// Had to not implement ObservableObject to make it serializable
    /// </summary>
    [DataContract()]
    public class PurchaseModel : INotifyPropertyChanged {
        public PurchaseModel() { }

        private bool isComplete = false;
        public bool IsComplete {
            get => isComplete;
            set => SetProperty(ref isComplete, value);
        }

        private DateTime lastUpdate = DateTime.Now;
        public DateTime LastUpdate {
            get => lastUpdate;
            set => SetProperty(ref lastUpdate, value);
        }

        [DataMember()]
        public string Crypto { get; set; }

        [DataMember()]
        public string CryptoLogo { get; set; }

        private double cryptoQty;
        [DataMember()]
        public double CryptoQty {
            get => cryptoQty;
            set => SetProperty(ref cryptoQty, value);
        }

        private DateTimeOffset date = DateTime.Today;
        [DataMember()]
        public DateTimeOffset Date {
            get => date;
            set => SetProperty(ref date, value);
        }

        private string exchange = "";
        [DataMember()]
        public string Exchange {
            get => exchange;
            set => SetProperty(ref exchange, value);
        }

        private string notes = "";
        [DataMember()]
        public string Notes {
            get => notes;
            set => SetProperty(ref notes, value);
        }

        private double investedQty = 0;
        [DataMember()]
        public double InvestedQty {
            get => investedQty;
            set => SetProperty(ref investedQty, value);
        }

        private double delta = 0;
        public double Delta {
            get => delta;
            set => SetProperty(ref delta, value);
        }

        private double boughtAt = 0;
        public double BoughtAt {
            get => boughtAt;
            set => SetProperty(ref boughtAt, value);
        }

        private string currency = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.Currency);
        [DataMember()]
        public string Currency {
            get => currency;
            set => SetProperty(ref currency, value);
        }

        private string arrow = "▲";
        public string Arrow {
            get => arrow;
            set => SetProperty(ref arrow, value);
        }


        private SolidColorBrush profitFG = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
        public SolidColorBrush ProfitFG {
            get => profitFG;
            set => SetProperty(ref profitFG, value);
        }

        private double current = 0;
        public double Current {
            get => current;
            set => SetProperty(ref current, value);
        }

        private double profit = 0;
        public double Profit {
            get => profit;
            set => SetProperty(ref profit, value);
        }

        private double worth = 0;
        public double Worth {
            get => worth;
            set => SetProperty(ref worth, value);
        }

        /// <summary>
        /// Similar SetProperty to that of the WCT's MVVM approach
        /// </summary>
        private void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null) {
            if (newValue == null)
                return;
            if (field == null || !newValue.Equals(field)) {
                field = newValue;
                NotifyPropertyChanged(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
