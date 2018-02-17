using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker {

    public partial class Portfolio : Page {

        static ObservableCollection<PurchaseClass> dataList { get; set; }
        private double curr = 0;

        public Portfolio() {
            this.InitializeComponent();

            dataList = ReadPortfolio().Result;
            MyListView.ItemsSource = dataList;

            UpdatePortfolio();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void AddButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            string crypto = ((ComboBoxItem)CryptoComboBox.SelectedItem).Content.ToString();

            curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);

            try {
                double priceBought = (1 / double.Parse(cryptoQtyTextBox.Text)) * double.Parse(investedQtyTextBox.Text);
                priceBought = Math.Round(priceBought, 2);
                double earningz = Math.Round((curr - priceBought) * double.Parse(cryptoQtyTextBox.Text), 5);

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

                UpdateEarnings();
                SavePortfolio();

            } catch(Exception) {
                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;
            }
        }

        //For Sync all
        internal void UpdatePortfolio() {

            for (int i = 0; i < MyListView.Items.Count; i++) {
                string crypto = dataList[i]._Crypto;

                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);

                dataList[i].Current = curr;
                double priceBought = (1 / dataList[i]._CryptoQty) * dataList[i]._InvestedQty;
                priceBought = Math.Round(priceBought, 2);
                dataList[i].Earnings = Math.Round((curr - priceBought) * dataList[i]._CryptoQty, 2).ToString();
            }
            UpdateEarnings();
            SavePortfolio();

            List<double> pie = new List<double>();

            if (MyListView.Items.Count == 0)
                pie.Add(100);

            for (int i = 0; i < MyListView.Items.Count; i++) {
                pie.Add( double.Parse(dataList[i].Earnings) );
            }

            portfolioPieChart.Series[0].ItemsSource = pie;

        }

        private void UpdateEarnings() {
            float total = 0;
            for (int i = 0; i < MyListView.Items.Count; i++) {
                total += float.Parse(dataList[i].Current.ToString()) * (float)dataList[i]._CryptoQty;
                Portfolio_total.Text = total.ToString() + App.coinSymbol;
            }
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

        private static async Task<ObservableCollection<PurchaseClass>> ReadPortfolio() {

            try {
                var readStream =
                    await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("portfolio").ConfigureAwait(false);

                DataContractSerializer stuffSerializer =
                    new DataContractSerializer(typeof(ObservableCollection<PurchaseClass>));

                var setResult = (ObservableCollection<PurchaseClass>)stuffSerializer.ReadObject(readStream);

                return setResult;
            } catch (Exception) {
                return new ObservableCollection<PurchaseClass>();
            }
        }

    }
}
