using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Helpers {
    [DataContractAttribute()]
    internal class PurchaseClass : INotifyPropertyChanged {

        [DataMember()]
        public string Exchange { get; set; }
        [DataMember()]
        public string Crypto { get; set; }
        [DataMember()]
        public double CryptoQty { get; set; }
        [DataMember()]
        public double InvestedQty { get; set; }
        [DataMember()]
        public double BoughtAt { get; set; }
        [DataMember()]
        public string c { get; set; }
        [DataMember()]
        public string arrow { get; set; }

        public SolidColorBrush ProfitFG { get; set; }

        private double _curr;
        [DataMember()]
        public double Current {
            get { return _curr; }
            set { if (value != _curr) { _curr = value; NotifyPropertyChanged("Current");}}}

        private string _profit;
        [DataMember()]
        public string Profit {
            get { return _profit; }
            set { if (value != _profit) { _profit = value; NotifyPropertyChanged("Profits"); } }}

        


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
