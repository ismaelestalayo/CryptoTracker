using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CoinBase.Helpers;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace CoinBase {

    public partial class Page_Portfolio : Page    {

        static ObservableCollection<PurchaseClass> dataList { get; set; }
        private double curr = 0;

        public Page_Portfolio(){
            this.InitializeComponent();

            try {
                dataList = new ObservableCollection<PurchaseClass>(ReadPortfolio().Result);

            } catch {
                dataList = new ObservableCollection<PurchaseClass>();
            }
            MyListView.ItemsSource = dataList;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            string crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();
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

            dataList.Add(new PurchaseClass {
                _Crypto      = crypto,
                _CryptoQty   = double.Parse( cryptoQtyTextBox.Text),
                _InvestedQty = double.Parse( investedQtyTextBox.Text),
                _BoughtAt    = Math.Round(priceBought, 2),
                c            = App.coinSymbol,
                upDown       = (earningz < 0 ? "▼" : "▲"),
                Current      = curr,
                Earnings     = Math.Abs(earningz).ToString()
                //earningsFG = (earningz < 0) ? new SolidColorBrush(Color.FromArgb(255, 180, 0, 0)) : new SolidColorBrush(Color.FromArgb(255, 0, 120, 0))
            });

            SavePortfolio();
        }

        //For Sync all
        internal async void UpdatePortfolio() {

            await App.GetCurrentPrice("BTC");
            await App.GetCurrentPrice("ETH");
            await App.GetCurrentPrice("LTC");

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
                dataList[i].Current = curr;
                double priceBought = (1 / dataList[i]._CryptoQty) * dataList[i]._InvestedQty;
                priceBought = Math.Round(priceBought, 3);
                dataList[i].Earnings = Math.Round((curr - priceBought), 3).ToString();
            }
            SavePortfolio();
            
        }
        private void RemovePortfolio_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            if (MyListView.SelectedIndex != -1) {
                dataList.RemoveAt(MyListView.SelectedIndex);
                SavePortfolio();
            }
        }



        private static async void SavePortfolio() {
            try {
                StorageFile savedStuffFile =
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("portfolio", CreationCollisionOption.ReplaceExisting);

                using (Stream writeStream =
                    await savedStuffFile.OpenStreamForWriteAsync()) {

                    DataContractSerializer stuffSerializer =
                        new DataContractSerializer(typeof(List<PurchaseClass>));

                    stuffSerializer.WriteObject(writeStream, dataList);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                    
                }
            } catch (Exception e){
                throw new Exception("ERROR saving portfolio", e);
            }
        }

        private static async Task<List<PurchaseClass>> ReadPortfolio() {
            var readStream =
                await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("portfolio");

            if (readStream == null)
                return new List<PurchaseClass>();

            DataContractSerializer stuffSerializer =
                new DataContractSerializer(typeof(List<PurchaseClass>));

            var setResult = (List<PurchaseClass>)stuffSerializer.ReadObject(readStream);
            return setResult;
        }
    }
}
