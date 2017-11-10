using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using CoinBase.Helpers;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml.Data;

public sealed class BackgroundConverter : IValueConverter {
    public object myConverter(object value, Type targetType, object parameter) {
        ListViewItem item = (ListViewItem)value;
        ListView listView =
            ItemsControl.ItemsControlFromItemContainer(item) as ListView;
        // Get the index of a ListViewItem
        int index =
            listView.ItemContainerGenerator.IndexFromContainer(item);

        if (index % 2 == 0) {
            return Colors.LightBlue;
        } else {
            return Colors.Beige;
        }
    }

    public object Convert(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        throw new NotImplementedException();
    }
}

namespace CoinBase {

    public partial class Page_Portfolio : Page {

        static ObservableCollection<PurchaseClass> dataList { get; set; }
        private double curr = 0;

        public Page_Portfolio() {
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
                    curr = Math.Round(App.BTC_now, 3);
                    break;
                case "ETH":
                    curr = Math.Round(App.ETH_now, 3);
                    break;
                case "LTC":
                    curr = Math.Round(App.LTC_now, 3);
                    break;
            }

            try {
                double priceBought = (1 / double.Parse(cryptoQtyTextBox.Text)) * double.Parse(investedQtyTextBox.Text);
                priceBought = Math.Round(priceBought, 2);
                double earningz = Math.Round((curr - priceBought) * double.Parse(cryptoQtyTextBox.Text), 4);

                dataList.Add(new PurchaseClass {
                    _Crypto = crypto,
                    _CryptoQty = Math.Round(double.Parse(cryptoQtyTextBox.Text), 5),
                    _InvestedQty = double.Parse(investedQtyTextBox.Text),
                    _BoughtAt = Math.Round(priceBought, 2),
                    c = App.coinSymbol,
                    upDown = (earningz < 0 ? "▼" : "▲"),
                    Current = curr,
                    Earnings = Math.Round(Math.Abs(earningz), 2).ToString()
                    //earningsFG = (earningz < 0) ? new SolidColorBrush(Color.FromArgb(255, 180, 0, 0)) : new SolidColorBrush(Color.FromArgb(255, 0, 120, 0))
                });

                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;

                SavePortfolio();

            } catch(Exception) {
                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;
            }
        }

        public object Convert(object value) {
            var val = (double)value;
            return val >= 0
                ? Colors.Green
                : Colors.Red;
        }

        //For Sync all
        internal async void UpdatePortfolio() {

            await App.GetCurrentPrice("BTC");
            await App.GetCurrentPrice("ETH");
            await App.GetCurrentPrice("LTC");

            for (int i = 0; i < MyListView.Items.Count; i++) {
                switch (dataList[i]._Crypto) {
                    case "BTC":
                        curr = Math.Round(App.BTC_now, 3);
                        break;
                    case "ETH":
                        curr = Math.Round(App.ETH_now, 3);
                        break;
                    case "LTC":
                        curr = Math.Round(App.LTC_now, 3);
                        break;
                }
                dataList[i].Current = curr;
                double priceBought = (1 / dataList[i]._CryptoQty) * dataList[i]._InvestedQty;
                priceBought = Math.Round(priceBought, 2);
                dataList[i].Earnings = Math.Round((curr - priceBought) * dataList[i]._CryptoQty, 2).ToString();
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
            } catch (Exception e) {
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
