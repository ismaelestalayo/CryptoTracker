using CryptoTracker.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {
    public partial class Portfolio : Page {

        static ObservableCollection<PurchaseClass> dataList { get; set; }
        private double curr = 0;

        public Portfolio() {
            this.InitializeComponent();

            CryptoComboBox.ItemsSource = App.coinList;

            dataList = ReadPortfolio().Result;
            portfolioListView.ItemsSource = dataList;
            dataGridd.ItemsSource = dataList;


            UpdatePortfolio();
            UpdateProfits();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        private void AddButton_Click(object sender, RoutedEventArgs e) {
            
            try {
                string crypto = CryptoComboBox.SelectedItem.ToString();
                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);


                double priceBought = (1 / double.Parse(cryptoQtyTextBox.Text)) * double.Parse(investedQtyTextBox.Text);
                priceBought = Math.Round(priceBought, 2);
                double earningz = Math.Round((curr - priceBought) * double.Parse(cryptoQtyTextBox.Text), 5);

                dataList.Add(new PurchaseClass {
                    Crypto     = crypto,
                    CryptoQty  = Math.Round(double.Parse(cryptoQtyTextBox.Text), 5),
                    InvestedQty= double.Parse(investedQtyTextBox.Text),
                    BoughtAt   = Math.Round(priceBought, 2),
                    arrow      = earningz < 0 ? "▼" : "▲",
                    c          = App.coinSymbol,
                    Current    = curr,
                    Profit     = Math.Round(Math.Abs(earningz), 2).ToString(),
                    ProfitFG   = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"]
                });

                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;

                UpdateProfits();
                SavePortfolio();

            } catch(Exception) {
                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;
            }
        }

        //For Sync all
        internal void UpdatePortfolio() {

            for (int i = 0; i < portfolioListView.Items.Count; i++) {
                string crypto = dataList[i].Crypto;

                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);

                dataList[i].Current = curr;
                double priceBought = (1 / dataList[i].CryptoQty) * dataList[i].InvestedQty;
                priceBought = Math.Round(priceBought, 2);

                double earningz = Math.Round((curr - priceBought) * dataList[i].CryptoQty, 2);
                dataList[i].arrow = earningz < 0 ? "▼" : "▲";
                dataList[i].Profit = earningz.ToString();
                dataList[i].ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
            }
            UpdateProfits();
            SavePortfolio();

            List<double> pie = new List<double>();

            if (portfolioListView.Items.Count == 0)
                pie.Add(100);

            for (int i = 0; i < portfolioListView.Items.Count; i++) {
                pie.Add( double.Parse(dataList[i].Profit) );
            }

            portfolioPieChart.Series[0].ItemsSource = pie;

        }

        private void UpdateProfits() {
            float total = 0;
            for (int i = 0; i < portfolioListView.Items.Count; i++) {
                total += float.Parse(dataList[i].Current.ToString()) * (float)dataList[i].CryptoQty;

                Frame contentFrame = Window.Current.Content as Frame;
                MainPage mp = contentFrame.Content as MainPage;
                TextBlock val = mp.FindName("mainTitleVal") as TextBlock;
                val.Text = total.ToString() + App.coinSymbol;
            }
        }

        private void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            if (portfolioListView.SelectedIndex != -1) {
                dataList.RemoveAt(portfolioListView.SelectedIndex);
                SavePortfolio();
            }
        }

        private void portfolioListView_ItemClick(object sender, ItemClickEventArgs e) {
            //if (portfolioListView.SelectedIndex != -1) {
            //    portfolioListView.SelectedIndex = -1;
            //}
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
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
