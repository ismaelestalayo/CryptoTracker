using CommunityToolkit.Mvvm.ComponentModel;

namespace UWP.Models {
    /// <summary>
    /// Class that holds user Alerts
    /// </summary>
    public partial class Alert : ObservableObject {
        [ObservableProperty]
        private bool enabled = false;

        public int Id { get; set; }


        [ObservableProperty]
        private string crypto = "";


        [ObservableProperty]
        private string currency = "";


        [ObservableProperty]
        private string currencySymbol = "";


        /// <summary>
        /// "above" or "below"
        /// </summary>
        /// TODO: use enum
        [ObservableProperty]
        private string mode = "above";


        private double threshold = 0;
        public double Threshold {
            get => threshold;
            set => SetProperty(ref threshold, double.IsNaN(value) ? 0 : value);
        }
    }
}