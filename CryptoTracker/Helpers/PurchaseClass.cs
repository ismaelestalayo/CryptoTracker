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
        public string upDown { get; set; }

        public double _Current;
        [DataMember()]
        public double Current {
            get { return _Current; }
            set { if (value != _Current) { _Current = value; NotifyPropertyChanged("Current");}}}
        public string _Earnings;
        [DataMember()]
        public string Earnings {
            get { return _Earnings; }
            set { if (value != _Earnings) { _Earnings = value; NotifyPropertyChanged("Earnings"); } }}

        public SolidColorBrush earningsFG { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
