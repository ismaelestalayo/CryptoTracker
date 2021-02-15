using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Models {
	[DataContractAttribute()]
    public class PurchaseModel : ObservableObject {

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

        private string exchange;
        [DataMember()]
        public string Exchange {
            get => exchange;
            set => SetProperty(ref exchange, value);
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

        private string currency = App.currencySymbol;
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
    }
}
