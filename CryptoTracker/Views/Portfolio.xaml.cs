using CryptoTracker.Helpers;
using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
        internal ObservableCollection<PurchaseClass> NewPurchase { get; set; }
        internal static List<string> coinsArray = App.coinList.Select(x => x.Name).ToList();
        private int EditingPurchaseId { get; set; }

        private double curr = 0;

        public Portfolio() {
            this.InitializeComponent();

            PurchaseList = ReadPortfolio().Result;
            DataGridd.ItemsSource = PurchaseList;

            UpdatePortfolio();
        }


        // ###############################################################################################
        //  For sync all
        internal void UpdatePortfolio() {
            // empty diversification chart
            PortfolioChartGrid.ColumnDefinitions.Clear();
            PortfolioChartGrid.Children.Clear();

            foreach (PurchaseClass purchase in PurchaseList) {
                // this update the ObservableCollection itself
                UpdatePurchase(purchase);

                // create the diversification grid
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(purchase.Worth, GridUnitType.Star);
                PortfolioChartGrid.ColumnDefinitions.Add(col);

                var s = new StackPanel();
                s.BorderThickness = new Thickness(0);
                s.Margin = new Thickness(1, 0, 1, 0);
                var t = new TextBlock() {
                    Text = purchase.Crypto,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 7, 0, 7)
                };
                s.Children.Add(t);
                try { s.Background = (SolidColorBrush)App.Current.Resources[purchase.Crypto + "_colorT"]; }
                catch { s.Background = (SolidColorBrush)App.Current.Resources["null_color"]; }

                PortfolioChartGrid.Children.Add(s);
                Grid.SetColumn(s, PortfolioChartGrid.Children.Count - 1);
            }
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
                purchase.BoughtAt = priceBought;
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(Math.Abs(earningz), 2);
                purchase.ProfitFG = (earningz < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
            }
            
            return purchase;
        }

        // ###############################################################################################
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
        //  Read/Write portfolio
        private static async void SavePortfolio() {
            try {
                StorageFile savedStuffFile =
                    await ApplicationData.Current.LocalFolder.CreateFileAsync("portfolio", CreationCollisionOption.ReplaceExisting);

                using (Stream writeStream =
                    await savedStuffFile.OpenStreamForWriteAsync().ConfigureAwait(false) ) {

                    DataContractSerializer stuffSerializer =
                        new DataContractSerializer(typeof(ObservableCollection<PurchaseClass>));

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

        internal static void importPortfolio(ObservableCollection<PurchaseClass>portfolio) {
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

        // ###############################################################################################
        // Add/Edit purchase dialog
        private void AddPurchase_click(object sender, RoutedEventArgs e) {
            NewPurchase = new ObservableCollection<PurchaseClass>() { new PurchaseClass() };
            TestRepeater.ItemsSource = NewPurchase;
            PurchaseDialog.Title = "💵 New purchase";
            PurchaseDialog.PrimaryButtonText = "Add";
            PurchaseDialog.ShowAsync();
        }

        private void EditPurchase_Click(object sender, RoutedEventArgs e) {
            var purchase = ((PurchaseClass)((FrameworkElement)sender).DataContext);
            EditingPurchaseId = PurchaseList.IndexOf(purchase);
            NewPurchase = new ObservableCollection<PurchaseClass>() { purchase };
            TestRepeater.ItemsSource = NewPurchase;
            PurchaseDialog.Title = "💵 Edit purchase";
            PurchaseDialog.PrimaryButtonText = "Save";
            PurchaseDialog.ShowAsync();
        }

        private void PurchaseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(NewPurchase[0].Crypto) || NewPurchase[0].CryptoQty <= 0 || NewPurchase[0].InvestedQty <= 0) {
                args.Cancel = true;
                new MessageDialog("Error.").ShowAsync();
            }
            else {
                if (sender.PrimaryButtonText == "Add") {
                    // Get logo for the coin
                    var crypto = NewPurchase[0].Crypto;
                    string logoURL = "Assets/Icons/icon" + crypto + ".png";
                    if (!File.Exists(logoURL))
                        NewPurchase[0].CryptoLogo = "https://chasing-coins.com/coin/logo/" + crypto;
                    else
                        NewPurchase[0].CryptoLogo = "/" + logoURL;
                    PurchaseList.Add(NewPurchase[0]);
                }
                else if(sender.PrimaryButtonText == "Save") {
                    PurchaseList.RemoveAt(EditingPurchaseId);
                    PurchaseList.Insert(EditingPurchaseId, NewPurchase[0]);
                }
                // Update and save
                UpdatePortfolio();
                SavePortfolio();
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

        private void DataGrid_Sorting(object sender, DataGridColumnEventArgs e) {
            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                e.Column.SortDirection = DataGridSortDirection.Ascending;
            else
                e.Column.SortDirection = DataGridSortDirection.Descending;

            switch (e.Column.Header) {
                case "Crypto":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Crypto ascending
                                                                                        select item);
                    else
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Crypto descending
                                                                                        select item);
                    break;
                case "Invested":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.InvestedQty ascending
                                                                                        select item);
                    else
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.InvestedQty descending
                                                                                        select item);
                    break;
                case "Worth":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Worth ascending
                                                                                        select item);
                    else
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Worth descending
                                                                                        select item);
                    break;
                case "Currently":
                    if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Descending)
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Current ascending
                                                                                        select item);
                    else 
                        DataGridd.ItemsSource = new ObservableCollection<PurchaseClass>(from item in PurchaseList
                                                                                        orderby item.Current descending
                                                                                        select item);
                    break;
            }
            foreach (var dgColumn in DataGridd.Columns) {
                if (dgColumn.Header.ToString() != e.Column.Header.ToString())
                    dgColumn.SortDirection = null;
            }

            /*
            if (!e.Column.SortDirection.HasValue) {
                e.Column.SortDirection = DataGridSortDirection.Ascending;
                //PurchaseList = PurchaseList.OrderBy(x => x.Crypto).ToList();
            }
            else {
                e.Column.SortDirection = DataGridSortDirection.Descending;
                //PurchaseList = PurchaseList.OrderByDescending(x => x.Crypto).ToList();
            }
            //DataGridd.ItemsSource = PurchaseList;
            */
        }
    }
}
