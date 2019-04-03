using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {

    public class SuggestionCoinList {
        public string Icon { get; set; }
        public string Name { get; set; }
    }

    public partial class Portfolio : Page {

        static ObservableCollection<PurchaseClass> dataList { get; set; }
        private double curr = 0;
 

        /*
        // ###############################################################################################
        public Portfolio() {
            this.InitializeComponent();

            List<SuggestionCoinList> coinList = new List<SuggestionCoinList>();
            foreach (JSONcoins coin in App.coinList)
                coinList.Add( new SuggestionCoinList {
                    Icon = IconsHelper.GetIcon(coin.Name),
                    Name = coin.Name
                });
            CryptoComboBox.ItemsSource = coinList;

            dataList = ReadPortfolio().Result;
            dataGridd.ItemsSource = dataList;


            UpdatePortfolio();
            UpdateProfits();
        } 
         */

        public Portfolio() {
            this.InitializeComponent();

            List<string> coinsArray = new List<string>();
            foreach (JSONcoins coin in App.coinList)
                coinsArray.Add(coin.Name);
            CryptoComboBox.ItemsSource = coinsArray;

            dataList = ReadPortfolio().Result;
            dataGridd.ItemsSource = dataList;


            UpdatePortfolio();
            UpdateProfits();
        }

        // ###############################################################################################
        //  Add new purchase
        private void AddButton_Click(object sender, RoutedEventArgs e) {
            
            try {
                string crypto = CryptoComboBox.SelectedItem.ToString();
                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);

                // Calculate earnings/losings
                double priceBought = (1 / double.Parse(cryptoQtyTextBox.Text)) * double.Parse(investedQtyTextBox.Text);
                priceBought = Math.Round(priceBought, 2);
                double earningz = Math.Round((curr - priceBought) * double.Parse(cryptoQtyTextBox.Text), 5);

                // Get logo for the coin
                string logoURL = "Assets/Icons/icon" + crypto + ".png";
                if (!File.Exists(logoURL))
                    logoURL = "https://chasing-coins.com/coin/logo/" + crypto;
                else
                    logoURL = "/" + logoURL;
                

                dataList.Add(new PurchaseClass {
                    Crypto      = crypto,
                    CryptoLogo  = logoURL,
                    CryptoQty   = Math.Round(double.Parse(cryptoQtyTextBox.Text), 5),
                    Date        = DateTime.Today,
                    Delta       = Math.Round( (curr / priceBought), 2) * 100, // percentage
                    InvestedQty = double.Parse(investedQtyTextBox.Text),
                    BoughtAt    = Math.Round(priceBought, 2),
                    arrow       = earningz < 0 ? "▼" : "▲",
                    c           = App.coinSymbol,
                    Current     = curr,
                    Profit      = Math.Round(Math.Abs(earningz), 2).ToString(),
                    ProfitFG    = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"],
                    Worth       = Math.Round( curr * double.Parse(cryptoQtyTextBox.Text), 2)
                });

                // Clear user input
                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;

                // Update and save
                UpdateProfits();
                SavePortfolio();

            } catch(Exception) {
                cryptoQtyTextBox.Text = String.Empty;
                investedQtyTextBox.Text = String.Empty;
            }
        }

        // ###############################################################################################
        //  For sync all
        internal void UpdatePortfolio() {

            for (int i = 0; i < ((Collection<PurchaseClass>)dataGridd.ItemsSource).Count; i++) {
                string crypto = dataList[i].Crypto;

                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 3);

                dataList[i].Current = curr;
                double priceBought = (1 / dataList[i].CryptoQty) * dataList[i].InvestedQty;
                priceBought = Math.Round(priceBought, 2);

                double earningz = Math.Round((curr - priceBought) * dataList[i].CryptoQty, 2);
                dataList[i].arrow = earningz < 0 ? "▼" : "▲";
                dataList[i].Delta = Math.Round(curr / priceBought, 2) * 100; // percentage
                dataList[i].Profit = earningz.ToString();
                dataList[i].ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
            }
            UpdateProfits();
            SavePortfolio();

            List<double> pie = new List<double>();
            
            for (int i = 0; i < ((Collection<PurchaseClass>)dataGridd.ItemsSource).Count; i++) {
                pie.Add( double.Parse(dataList[i].Profit) );
            }

            portfolioPieChart.Series[0].ItemsSource = pie;

        }

        // ###############################################################################################
        private void UpdateProfits() {
            float total = 0;
            for (int i = 0; i < ((Collection<PurchaseClass>)dataGridd.ItemsSource).Count; i++) {
                total += float.Parse(dataList[i].Current.ToString()) * (float)dataList[i].CryptoQty;

                Frame contentFrame = Window.Current.Content as Frame;
                MainPage mp = contentFrame.Content as MainPage;
                //TextBlock val = mp.FindName("mainTitleVal") as TextBlock;
                //val.Text = total.ToString() + App.coinSymbol;
            }
        }

        private void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            if (dataGridd.SelectedIndex != -1) {
                dataList.RemoveAt(dataGridd.SelectedIndex);
                SavePortfolio();
            }
        }

        // ###############################################################################################
        //  Save new purchase date
        private void purchaseDate_changed(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args) {
            if (args.OldDate > new DateTime(2000, 01, 01, 00, 00, 00, 0) ) 
                SavePortfolio();
            
        }
        


        // ###############################################################################################
        //  Read/Write portfolio
        private static async void SavePortfolio() {
            try {
                StorageFile savedStuffFile =
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("portfolio", CreationCollisionOption.ReplaceExisting);

                using (Stream writeStream =
                    await savedStuffFile.OpenStreamForWriteAsync().ConfigureAwait(false) ) {

                    DataContractSerializer stuffSerializer =
                        new DataContractSerializer(typeof(List<PurchaseClass>));

                    stuffSerializer.WriteObject(writeStream, dataList);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();

                }
            } catch (Exception e) {
                var z = e.Message;
            }
        }
        private static async Task<ObservableCollection<PurchaseClass>> ReadPortfolio() {

            try {
                var readStream =
                    await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("portfolio").ConfigureAwait(false);

                DataContractSerializer stuffSerializer =
                    new DataContractSerializer(typeof(ObservableCollection<PurchaseClass>));

                var setResult = (ObservableCollection<PurchaseClass>)stuffSerializer.ReadObject(readStream);
                await readStream.FlushAsync();
                readStream.Dispose();

                return setResult;
            } catch (Exception ex) {
                var unusedWarning = ex.Message;
                return new ObservableCollection<PurchaseClass>();
            }
        }

        
    }
}
