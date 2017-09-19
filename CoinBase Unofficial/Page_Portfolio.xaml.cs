using System;
using System.Collections.ObjectModel;
using CoinBase.Helpers;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {

    public partial class Page_Portfolio : Page    {

        ObservableCollection<PurchaseClass> dataList { get; set; }

        public Page_Portfolio(){
            this.InitializeComponent();

            dataList = new ObservableCollection<PurchaseClass>();
            MyListView.ItemsSource = dataList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            string crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();
            double curr = 0;
            switch (crypto) {
                case "BTC":
                    curr = Math.Round( App.BTC_now, 3);
                    break;
                case "ETH":
                    curr = Math.Round( App.ETH_now, 3);
                    break;
                case "LTC":
                    curr = Math.Round( App.LTC_now, 3);
                    break;
            }

            double priceBought = (1 / double.Parse(cryptoQtyTextBox.Text)) * double.Parse(investedQtyTextBox.Text);
            priceBought = Math.Round(priceBought, 3);
            double earningz = Math.Round( curr - priceBought, 3);
            string c = ((App.coin.Equals("EUR")) ? "€" : "$");

            dataList.Add(new PurchaseClass {
                _Crypto      = crypto,
                _CryptoQty   = double.Parse( cryptoQtyTextBox.Text),
                _InvestedQty = double.Parse( investedQtyTextBox.Text),
                _BoughtAt    = Math.Round(priceBought, 2),
                Earnings     = (earningz < 0 ? "▼" : "▲") + Math.Abs(earningz).ToString() + c
                //earningsFG = (earningz < 0) ? new SolidColorBrush(Color.FromArgb(255, 180, 0, 0)) : new SolidColorBrush(Color.FromArgb(255, 0, 120, 0))
            });

        }

        //For Sync all
        internal async void UpdatePortfolio() {

            await App.GetCurrentPrice("BTC");
            await App.GetCurrentPrice("ETH");
            await App.GetCurrentPrice("LTC");

            double curr = 0;

            for (int i = 0; i < MyListView.Items.Count; i++) {
                switch (dataList[i]._Crypto) {
                    case "BTC":
                        curr = Math.Round( App.BTC_now, 3);
                        break;
                    case "ETH":
                        curr = Math.Round( App.ETH_now, 3);
                        break;
                    case "LTC":
                        curr = Math.Round( App.LTC_now, 3);
                        break;
                }
                dataList[i].Current = curr.ToString();
                double priceBought = (1 / float.Parse(dataList[i].CryptoAmount)) * float.Parse(dataList[i].Invested);
                priceBought = Math.Round(priceBought, 3);
                dataList[i].Earnings = Math.Round((curr - priceBought), 3).ToString();
            }
            
        }
        private void RemovePortfolio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if(MyListView.SelectedIndex != -1)
                dataList.RemoveAt(MyListView.SelectedIndex);
        }

        private void SavePortfolio() {
            
        }



    }
}
