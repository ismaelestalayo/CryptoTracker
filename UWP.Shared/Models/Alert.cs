using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace UWP.Models {
    public class Alert : ObservableObject {
        /// <summary>
        /// Class that holds user Alerts
        /// </summary>
        private bool enabled = false;
        public bool Enabled {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        public int Id { get; set; }

        private string crypto = "";
        public string Crypto {
            get => crypto;
            set => SetProperty(ref crypto, value);
        }

        private string currency = "";
        public string Currency {
            get => currency;
            set => SetProperty(ref currency, value);
        }

        private string currencySymbol = "";
        public string CurrencySymbol {
            get => currencySymbol;
            set => SetProperty(ref currencySymbol, value);
        }

        private string mode = "";
        public string Mode {
            get => mode;
            set => SetProperty(ref mode, value);
        }

        private double threshold = 0;
        public double Threshold {
            get => threshold;
            set => SetProperty(ref threshold, double.IsNaN(value) ? 0 : value);
        }
    }
}
