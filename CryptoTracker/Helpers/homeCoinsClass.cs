using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Telerik.UI.Xaml.Controls.Chart;

namespace CryptoTracker.Helpers {

    [DataContractAttribute()]
    internal class homeCoinsClass : INotifyPropertyChanged {

        public string _cryptoName { get; set; }
        public string _priceCurr { get; set; }
        public string _priceDiff { get; set; }
        public RadCartesianChart _chart { get; set; }
        public SplineAreaSeries _splineAreaSeries { get; set; }
        public SplineAreaSeries _testSource { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) {
                System.Diagnostics.Debug.WriteLine("Update now");
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
