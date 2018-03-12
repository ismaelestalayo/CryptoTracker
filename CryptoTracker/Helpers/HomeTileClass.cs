using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoTracker.Helpers {
    internal class HomeTileClass : INotifyPropertyChanged{

        public event PropertyChangedEventHandler PropertyChanged;

        public string _cryptoName { get; set; }
        public string _crypto { get; set; }
        public string _iconSrc { get; set; }
        public string _timeSpan { get; set; }
        public int _limit { get; set; }

        public string curr;
        public string _priceCurr {
            get { return curr; }
            set {
                curr = value;
                NotifyPropertyChanged("Curr");
            }
        }
        private string diff;
        public string _priceDiff {
            get { return diff; }
            set {
                diff = value;
                NotifyPropertyChanged("Diff");
            }
        }

        public void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
