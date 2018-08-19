using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Helpers {
    [DataContractAttribute()]
    internal class PurchaseClass : INotifyPropertyChanged {

        [DataMember()]
        public string _Crypto { get; set; }
        [DataMember()]
        public double _CryptoQty { get; set; }
        [DataMember()]
        public double _InvestedQty { get; set; }
        [DataMember()]
        public double _BoughtAt { get; set; }
        [DataMember()]
        public string c { get; set; }
        [DataMember()]
        public string _arrow { get; set; }

        private double _Current;
        [DataMember()]
        public double Current {
            get { return _Current; }
            set { if (value != _Current) { _Current = value; NotifyPropertyChanged("Current");}}}
        private string _Profits;
        [DataMember()]
        public string Profit {
            get { return _Profits; }
            set { if (value != _Profits) { _Profits = value; NotifyPropertyChanged("Profits"); } }}

        public SolidColorBrush ProfitFG { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
