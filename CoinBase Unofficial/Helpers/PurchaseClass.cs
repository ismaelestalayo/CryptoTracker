using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CoinBase.Helpers {
    [Serializable()]
    internal class PurchaseClass : INotifyPropertyChanged {

        [DataMember()]
        public string _Crypto { get; set; }
        [DataMember()]
        public double _CryptoQty { get; set; }
        [DataMember()]
        public double _InvestedQty { get; set; }
        [DataMember()]
        public double _BoughtAt { get; set; }


        public string CryptoAmount { get; set; }
        public string Invested { get; set; }
        public string BoughtAt { get; set; }
        public string _Current;
        public string Current {
            get { return _Current; }
            set {
                if (value != _Current) {
                    _Current = value;
                    NotifyPropertyChanged("Current");
                }

            }
        }
        public string _Earnings;
        public string Earnings {
            get { return _Earnings; }
            set {
                if (value != _Earnings) {
                    _Earnings = value;
                    NotifyPropertyChanged("Earnings");
                }

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            System.Diagnostics.Debug.WriteLine("Shortly before update. PropertyName = " + propertyName);
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
