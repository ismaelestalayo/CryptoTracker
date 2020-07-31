using System.ComponentModel;

namespace CryptoTracker.Model {
    internal class HomeTile : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string _cryptoName { get; set; }
        public string _crypto { get; set; }
        public string _iconSrc { get; set; }
        public string _timeSpan { get; set; }
        public int _limit { get; set; }

        private double alpha { get; set; }
        public double _opacity {
            get { return alpha; }
            set {
                alpha = value;
                RaiseProperty(nameof(_opacity));
            }
        }

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
                if (value != diff) {
                    diff = value;
                    if (value.StartsWith("▼"))
                        _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                    else
                        _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];

                    RaiseProperty(nameof(_priceDiff));
                }
            }
        }

        private string vol24;
        public string _volume24 {
            get { return vol24; }
            set {
                if (value != vol24) {
                    vol24 = value;
                    RaiseProperty(nameof(_volume24));
                }
            }
        }

        private string vol24to;
        public string _volume24to {
            get { return vol24to; }
            set {
                if (value != vol24to) {
                    vol24to = value;
                    RaiseProperty(nameof(_volume24to));
                }
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
}
