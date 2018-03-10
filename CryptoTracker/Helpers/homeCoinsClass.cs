using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace CryptoTracker.Helpers {

    [DataContractAttribute()]
    internal class HomeCoinsClass : INotifyPropertyChanged{

        public string _cryptoName { get; set; }
        public string _priceDiff { get; set; }
        public string _crypto { get; set; }
        public string _iconSrc { get; set; }
        public string curr;
        public string _priceCurr {
            get { return curr; }
            set { if (value != curr) { curr = value; NotifyPropertyChanged("Current"); } }
        }


        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
