using CryptoTracker.Helpers;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {

    public class SuggestionCoinList {
        public string Icon { get; set; }
        public string Name { get; set; }
    }

    public partial class Portfolio : Page {


        internal static ObservableCollection<PurchaseClass> PurchaseList { get; set; }
        internal List<PurchaseClass> NewPurchase { get; set; }
        internal static List<string> coinsArray = App.coinList.Select(x => x.Name).ToList();
        //internal static List<string> fullCoinList = App.coinList.Select(x => x.Name).ToList();

        private double curr = 0;

        public Portfolio() {
            this.InitializeComponent();

            PurchaseList = ReadPortfolio().Result;
            DataGridd.ItemsSource = PurchaseList;

            UpdatePortfolio();
        }

        

        // ###############################################################################################
        //  Add new purchase
        //private void AddButton_Click(object sender, RoutedEventArgs e) {
            
        //    try {
        //        string crypto = CryptoComboBox.SelectedItem.ToString();
        //        curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 4);

        //        // Calculate earnings/losings
        //        double priceBought = (1 / CryptoQtyNumberBox.Value) * InvestedQtyNumberBox.Value;
        //        priceBought = Math.Round(priceBought, 4);
        //        double earningz = Math.Round((curr - priceBought) * CryptoQtyNumberBox.Value, 5);

        //        // Get logo for the coin
        //        string logoURL = "Assets/Icons/icon" + crypto + ".png";
        //        if (!File.Exists(logoURL))
        //            logoURL = "https://chasing-coins.com/coin/logo/" + crypto;
        //        else
        //            logoURL = "/" + logoURL;
                

        //        PurchaseList.Add(new PurchaseClass {
        //            Crypto      = crypto,
        //            CryptoLogo  = logoURL,
        //            CryptoQty   = Math.Round(CryptoQtyNumberBox.Value, 5),
        //            Date        = DateTime.Today,
        //            Delta       = Math.Round( (curr / priceBought), 2) * 100, // percentage
        //            InvestedQty = InvestedQtyNumberBox.Value,
        //            BoughtAt    = Math.Round(priceBought, 4),
        //            arrow       = earningz < 0 ? "▼" : "▲",
        //            c           = App.coinSymbol,
        //            Current     = curr,
        //            Profit      = Math.Round(Math.Abs(earningz), 2).ToString(),
        //            ProfitFG    = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"],
        //            Worth       = Math.Round( curr * CryptoQtyNumberBox.Value, 2)
        //        });                

        //        // Update and save
        //        UpdatePortfolio();
        //        SavePortfolio();

        //    } catch(Exception) {
        //        //CryptoQtyTextBox.Text = String.Empty;
        //        //InvestedQtyTextBox.Text = String.Empty;
        //    }
        //}

        // ###############################################################################################
        //  For sync all
        internal void UpdatePortfolio() {

            for (int i = 0; i < ((Collection<PurchaseClass>)DataGridd.ItemsSource).Count; i++) {
                string crypto = PurchaseList[i].Crypto;

                curr = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 4);

                PurchaseList[i].Current = curr;
                double priceBought = (1 / PurchaseList[i].CryptoQty) * PurchaseList[i].InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * PurchaseList[i].CryptoQty, 4);
                PurchaseList[i].arrow = earningz < 0 ? "▼" : "▲";
                PurchaseList[i].Delta = Math.Round(curr / priceBought, 2) * 100;
                if (PurchaseList[i].Delta > 100)
                    PurchaseList[i].Delta -= 100;
                PurchaseList[i].Profit = Math.Round(Math.Abs(earningz), 2).ToString();
                PurchaseList[i].ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
                PurchaseList[i].Worth = Math.Round(curr * PurchaseList[i].CryptoQty, 2);
            }
            UpdateProfits();
            SavePortfolio();
        }

        internal PurchaseClass UpdatePurchase(PurchaseClass purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = Math.Round(App.GetCurrentPrice(crypto, "defaultMarket"), 4);

            curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            // If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty > 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                purchase.arrow = earningz < 0 ? "▼" : "▲";
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(Math.Abs(earningz), 2).ToString();
                purchase.ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];

            }
            
            return purchase;
        }

        // ###############################################################################################
        private void UpdateProfits() {
            float total = 0;
            try {
                // empty chart
                PortfolioChartGrid.ColumnDefinitions.Clear();
                PortfolioChartGrid.Children.Clear();

                for (int i = 0; i < ((Collection<PurchaseClass>)DataGridd.ItemsSource).Count; i++) {
                    total += float.Parse(PurchaseList[i].Current.ToString()) * (float)PurchaseList[i].CryptoQty;


                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(PurchaseList[i].Worth, GridUnitType.Star);
                    PortfolioChartGrid.ColumnDefinitions.Add(col);

                    var s = new StackPanel();
                    s.BorderThickness = new Thickness(0);
                    s.Margin = new Thickness(1, 0, 1, 0);
                    var t = new TextBlock();
                    t.Text = PurchaseList[i].Crypto;
                    t.FontSize = 12;
                    t.HorizontalAlignment = HorizontalAlignment.Center;
                    t.Margin = new Thickness(0, 7, 0, 7);
                    s.Children.Add(t);
                    try { s.Background = (SolidColorBrush)App.Current.Resources[PurchaseList[i].Crypto + "_colorT"]; }
                    catch { s.Background = (SolidColorBrush)App.Current.Resources["null_color"]; }

                    PortfolioChartGrid.Children.Add(s);
                    Grid.SetColumn(s, i);
                }
            } catch (Exception ex){
                Analytics.TrackEvent(string.Format("UpdateProfits({0})", ex));
            }
            

        }

        private void GoToCoinPortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseClass;
            this.Frame.Navigate(typeof(CoinDetails), item.Crypto);
        }

        private void RemovePortfolio_Click(object sender, RoutedEventArgs e) {
            var menu = sender as MenuFlyoutItem;
            var item = menu.DataContext as PurchaseClass;
            var items = DataGridd.ItemsSource.Cast<PurchaseClass>().ToList();
            var index = items.IndexOf(item);
            PurchaseList.RemoveAt(index);
            UpdatePortfolio();
            SavePortfolio();
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

                    stuffSerializer.WriteObject(writeStream, PurchaseList);
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

        internal static void importPortfolio(List<PurchaseClass>portfolio) {
            PurchaseList = new ObservableCollection<PurchaseClass>(portfolio);
            SavePortfolio();
        }

        private void ToggleDetails_click(object sender, RoutedEventArgs e) {
            if (DataGridd.RowDetailsVisibilityMode == Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowDetailsVisibilityMode.Visible) {
                DataGridd.RowDetailsVisibilityMode = Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowDetailsVisibilityMode.Collapsed;
                DataGridd.GridLinesVisibility = Microsoft.Toolkit.Uwp.UI.Controls.DataGridGridLinesVisibility.Horizontal;
            }
            else {
                DataGridd.RowDetailsVisibilityMode = Microsoft.Toolkit.Uwp.UI.Controls.DataGridRowDetailsVisibilityMode.Visible;
                DataGridd.GridLinesVisibility = Microsoft.Toolkit.Uwp.UI.Controls.DataGridGridLinesVisibility.Horizontal;
            }
        }

        private void AddPurchaseDialog_click(object sender, RoutedEventArgs e) {
            NewPurchase = new List<PurchaseClass>() { new PurchaseClass() };
            TestRepeater.ItemsSource = NewPurchase;
            TestDialog.ShowAsync();
        }

        private void TestDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(NewPurchase[0].Crypto) || NewPurchase[0].CryptoQty <= 0 || NewPurchase[0].InvestedQty <= 0) {
                args.Cancel = true;
                new MessageDialog("Error.").ShowAsync();
            }
            else {
                PurchaseList.Add(NewPurchase[0]);
            }
        }

        private void DialogBtn_LostFocus(object sender, RoutedEventArgs e) {
            // If we change the crypto, set the current price to 0 so everything updates
            if (sender.GetType().Name == "ComboBox")
                NewPurchase[0].Current = 0;

            // If we have the coin and the quantity, we can update some properties
            if (!string.IsNullOrEmpty(NewPurchase[0].Crypto) && NewPurchase[0].CryptoQty > 0)
                NewPurchase[0] = UpdatePurchase(NewPurchase[0]);
        }
    }
}
