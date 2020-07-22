using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Helpers {
    [DataContractAttribute()]
    internal class PurchaseClass : INotifyPropertyChanged {

        private string _exchange { get; set; }
        [DataMember()]
        public string Exchange {
            get { return _exchange; }
            set { _exchange = value; NotifyPropertyChanged(nameof(Exchange)); } }

        [DataMember()]
        public string Crypto { get; set; }

        [DataMember()]
        public string CryptoLogo { get; set; }

        private double _cryptoQty { get; set; }
        [DataMember()]
        public double CryptoQty {
            get { return _cryptoQty; }
            set { _cryptoQty = value; NotifyPropertyChanged(nameof(CryptoQty)); } }

        [DataMember()]
        public DateTimeOffset Date { get; set; } = DateTime.Today;

        private double _delta { get; set; }
        [DataMember()]
        public double Delta {
            get { return _delta; }
            set { _delta = value; NotifyPropertyChanged(nameof(Delta)); } }

        private double _investedQty { get; set; }
        [DataMember()]
        public double InvestedQty {
            get { return _investedQty; }
            set { _investedQty = value; NotifyPropertyChanged(nameof(InvestedQty)); } }

        private double _boughtAt { get; set; }
        [DataMember()]
        public double BoughtAt {
            get { return _boughtAt; }
            set { _boughtAt = value; NotifyPropertyChanged(nameof(BoughtAt)); } }

        private string _c { get; set; } = App.coinSymbol;
        [DataMember()]
        public string c {
            get { return _c; }
            set { _c = value; NotifyPropertyChanged(nameof(c)); } }

        private string _arrow { get; set; }
        [DataMember()]
        public string arrow {
            get { return _arrow; }
            set { _arrow = value; NotifyPropertyChanged(nameof(arrow)); } }


        private SolidColorBrush _profitFG { get; set; }
        public SolidColorBrush ProfitFG {
            get { return _profitFG; }
            set { _profitFG = value; NotifyPropertyChanged(nameof(ProfitFG)); } }

        private double _curr { get; set; } = 0;
        [DataMember()]
        public double Current {
            get { return _curr; }
            set { _curr = value; NotifyPropertyChanged(nameof(Current)); } }

        private string _profit { get; set; }
        [DataMember()]
        public string Profit {
            get { return _profit; }
            set { _profit = value; NotifyPropertyChanged(nameof(Profit)); } }

        private double _worth { get; set; } = 0;
        [DataMember()]
        public double Worth {
            get { return _worth; }
            set { _worth = value; NotifyPropertyChanged(nameof(Worth)); } }



        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propertyName = "") {
            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
}
