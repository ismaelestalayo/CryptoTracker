using System;
using System.Collections.ObjectModel;
using CoinBase.Helpers;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

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
            string _crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();
            string curr = "0";
            switch (_crypto) {
                case "BTC":
                    curr = App.BTC_now.ToString();
                    break;
                case "ETH":
                    curr = App.ETH_now.ToString();
                    break;
                case "LTC":
                    curr = App.LTC_now.ToString();
                    break;
            }

            float priceBought = (1 / float.Parse(_cryptoAmount.Text)) * float.Parse(_invested.Text);
            float earningz = float.Parse(curr) - priceBought;
            string c = ((App.coin.Equals("EUR")) ? "€" : "$");

            dataList.Add(new PurchaseClass {
                Crypto       = _crypto,
                CryptoAmount = _cryptoAmount.Text,
                Invested     = _invested.Text + c,
                BoughtAt     = Math.Round(priceBought, 2).ToString() + c,
                Earnings     = (earningz < 0 ? "▼" : "▲") + Math.Abs(earningz).ToString() + c
            });


        }

        private async void UpdatePortfolio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {

            await App.GetCurrentPrice("BTC");
            await App.GetCurrentPrice("ETH");
            await App.GetCurrentPrice("LTC");

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
            }
        }
        private void RemovePortfolio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if(MyListView.SelectedIndex != -1)
                dataList.RemoveAt(MyListView.SelectedIndex);
        }

        private void SavePortfolio() {
            // Composite setting
            var composite = new ApplicationDataCompositeValue();

            App.localSettings.Values["portfolio"] = composite;
        }
    }
}
