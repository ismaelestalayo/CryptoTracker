using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;

namespace CoinBase {

    class Purchase : INotifyPropertyChanged {

        public string Crypto { get; set; }
        public string Amount { get; set; }
        public string Invested { get; set; }
        public string _Current;
        public string Current {
            get { return _Current;  }
            set {
                if(value != _Current) {
                    _Current = value;
                    NotifyPropertyChanged("Current");
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

    public partial class Page_Portfolio : Page    {

        ObservableCollection<Purchase> dataList { get; set; }

        public Page_Portfolio(){
            this.InitializeComponent();

            dataList = new ObservableCollection<Purchase>();

            MyListView.ItemsSource = dataList;
        }

        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            string _crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();

            dataList.Add( new Purchase {
                Crypto = _crypto,
                Amount = _cryptoAmount.Text,
                Invested = _invested.Text,
                Current = App.LTC_now.ToString()
                //switch (_crypto) {
                //    case "BTC":
                //        Current = App.BTC_now.ToString();
                //        break;
                //    case "ETH":
                //        Current = App.ETH_now.ToString();
                //        break;
                //    case "LTC":
                //        Current = App.LTC_now.ToString();
                //        break;
                //}
            });

        }

        private void UpdatePortfolio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {

            for (int i = 0; i < MyListView.Items.Count; i++) {
                switch (dataList[i].Crypto) {
                    case "BTC":
                        dataList[i].Current = App.BTC_now.ToString();
                        break;
                    case "ETH":
                        dataList[i].Current = App.ETH_now.ToString();
                        break;
                    case "LTC":
                        dataList[i].Current = App.LTC_now.ToString();
                        break;
                }
                dataList[i].Current = App.LTC_now.ToString();
            }
        }

    }
}
