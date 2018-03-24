using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Helpers {
    internal class HomeTileClass : INotifyPropertyChanged{

        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
                RaiseProperty(nameof(_priceCurr));
            }
        }
        private string diff;
        public string _priceDiff {
            get { return diff; }
            set {
                diff = value;
                if (value.StartsWith("▼"))
                    _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                else
                    _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
                RaiseProperty(nameof(_priceDiff));
            }
        }
        private SolidColorBrush fg;
        public SolidColorBrush _priceDiffFG {
            get { return fg; }
            set {
                fg = value;
                RaiseProperty(nameof(_priceDiffFG));
            }
        }
    }
}
