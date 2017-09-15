using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;

namespace CoinBase {

    class Purchase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Crypto { get; set; }
        public string Amount { get; set; }
        public string Invested { get; set; }
        private string _Current = App.BTC_now.ToString();
        public string Current { get; set; }

    }

    public partial class Page_Portfolio : Page    {
        
        ObservableCollection<Purchase> dataList { get; set; }

        public Page_Portfolio(){
            this.InitializeComponent();

            MyListView.ItemsSource = dataList;
        }

        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            string _crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();

            dataList.Add( new Purchase {
                Crypto = _crypto,
                Amount = _cryptoAmount.Text,
                Invested = _invested.Text,
                Current = App.LTC_now.ToString()
            });

        }

        private void DeleteItem_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            //ListView v = MyListView;
            for (int i = 0; i < MyListView.Items.Count; i++) {
                dataList[i].Current = App.LTC_now.ToString();
            }
        }

    }
}
