using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace UWP.Models {
    public class Alert : ObservableObject {
        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
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

        /// <summary>
        /// Alert mode: 
        /// - 0: above
        /// - 1: below: 
        /// </summary>
        private int mode = 0;
        public int Mode {
            get => mode;
            set => SetProperty(ref mode, value);
        }

        private double threshold = 0;
        public double Threshold {
            get => threshold;
            set => SetProperty(ref threshold, value);
        }
	}
}
