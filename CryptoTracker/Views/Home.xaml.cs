using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
    public sealed partial class Home : Page {

        static ObservableCollection<HomeTileClass> homeCoinList { get; set; }
        private string diff = "0";
        private int limit = 1500;
        private string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();

            homeCoinList = new ObservableCollection<HomeTileClass>();
            priceListView.ItemsSource = homeCoinList;
            volumeListView.ItemsSource = homeCoinList;

            InitHome();
        }

        private async void InitHome() {
            // keep an updated list of coins
            await App.GetCoinList();

            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                try {
                    await AddCoinHome(App.pinnedCoins[i]);
                    await UpdateCard(i);
                } catch (Exception ex) {
                    var message = ex.Message;
                }
            }
        }

        // #########################################################################################
        // Add a new coin
        private async Task AddCoinHome(string c) {

            if (c == "MIOTA")
                c = "IOT";

            String iconPath = "/Assets/Icons/icon" + c + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch (Exception) {
                iconPath = "/Assets/Icons/iconNULL.png";
            }

            homeCoinList.Add(new HomeTileClass {
                _cryptoName = c,
                _priceDiff = diff,
                _crypto = c,
                _iconSrc = iconPath,
                _timeSpan = timeSpan,
                _limit = limit,
            });
        }

        // #########################################################################################
        //  Update all cards
        internal async Task UpdateAllCards() {
            for (int i = 0; i < homeCoinList.Count; i++) {
                await UpdateCard(i);
            }
        }

        internal async Task UpdateCard(int i) {
            
            string c = App.pinnedCoins[i];

            // DATA:
            await App.GetHisto(c, timeSpan, limit);

            float d = 0;
            float oldestPrice;
            float newestPrice;
            if (App.historic != null) {
                oldestPrice = App.historic[0].Close;
                newestPrice = App.historic[App.historic.Count - 1].Close;
            } else {
                oldestPrice = 0;
                newestPrice = 0;
            }

            d = (float)Math.Round(((newestPrice / oldestPrice) - 1) * 100, 2);

            if (d < 0) {
                d = Math.Abs(d);
                diff = "▼" + d.ToString() + "%";
            } else
                diff = "▲" + d.ToString() + "%";

            homeCoinList[i]._priceCurr = App.GetCurrentPrice(c, "defaultMarket").ToString() + App.coinSymbol;
            homeCoinList[i]._priceDiff = diff;

            await App.GetCoinStats(c, "defaultMarket");
            homeCoinList[i]._volume24 = App.stats.Volume24;
            homeCoinList[i]._volume24to = App.stats.Volume24To;

            // #########################################################################################
            // LOADING BAR
            ListViewItem container = (ListViewItem)priceListView.ContainerFromIndex(i);
            var loading = (container.ContentTemplateRoot as FrameworkElement)?.FindName("LoadingControl") as Loading;
            loading.IsLoading = true;

            // #########################################################################################
            // COLOR
            SolidColorBrush coinColorT, coinColor;
            try {
                coinColorT = (SolidColorBrush)Application.Current.Resources[c.ToUpper() + "_colorT"];
                coinColor = (SolidColorBrush)Application.Current.Resources[c.ToUpper() + "_color"];
            } catch (Exception) {
                coinColorT = (SolidColorBrush)Application.Current.Resources["null_colorT"];
                coinColor = (SolidColorBrush)Application.Current.Resources["null_color"];
            }

            // #########################################################################################
            // PRICE CHART
            //if (container == null)
            //    break;

            RadCartesianChart priceChart = (container.ContentTemplateRoot as FrameworkElement)?.FindName("priceChart") as RadCartesianChart;

            await App.GetHisto(c, timeSpan, limit);
            List<App.ChartDataObject> priceData = new List<App.ChartDataObject>();

            for (int k = 0; k < App.historic.Count; ++k) {
                priceData.Add(new App.ChartDataObject() {
                    Date = App.historic[k].DateTime,
                    Value = (App.historic[k].Low + App.historic[k].High) / 2,
                    Low = App.historic[k].Low,
                    High = App.historic[k].High,
                    Open = App.historic[k].Open,
                    Close = App.historic[k].Close,
                    Volume = App.historic[k].Volumefrom
                });
            }

            SplineAreaSeries series = (SplineAreaSeries)priceChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = priceData;
            series.Fill = coinColorT;
            series.Stroke = coinColor;

            // #########################################################################################
            // VOLUME CHART
            ListViewItem container2 = (ListViewItem)volumeListView.ContainerFromIndex(i);
            await App.GetHisto(c, "hour", 24);

            List<App.ChartDataObject> volumeData = new List<App.ChartDataObject>();
            for (int j = 0; j < 24; j++) {
                volumeData.Add(new App.ChartDataObject() {
                    Date = App.historic[j].DateTime,
                    Volume = App.historic[j].Volumefrom,
                    cc = coinColorT
                });
            }

            RadCartesianChart volumeChart = (container2.ContentTemplateRoot as FrameworkElement)?.FindName("volumeChart") as RadCartesianChart;
            BarSeries barSeries = (BarSeries)volumeChart.Series[0];
            barSeries.ItemsSource = volumeData;
            var z = barSeries.DefaultVisualStyle;



            loading.IsLoading = false;
            
        }

        // #########################################################################################
        private async void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    timeSpan = "minute";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "minute";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "hour";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "hour";
                    limit = 744;
                    break;
                case "year":
                    timeSpan = "day";
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "day";
                    limit = 0;
                    break;

            }

            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                await UpdateCard(i);
            }
        }

        private void homeListView_Click(object sender, ItemClickEventArgs e) {
            // Connected animation
            switch ( ((ListView)sender).Name ) {
                case "priceListView":
                    priceListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "priceListView_Element");
                    break;

                case "volumeListView":
                    volumeListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "volumeListView_Element");
                    break;
            }

            var clickedItem = (HomeTileClass)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem._crypto);
        }
    }



    // #########################################################################################
    // #########################################################################################
    // #########################################################################################
    // #########################################################################################
    internal class HomeTileClass : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string _cryptoName { get; set; }
        public string _crypto { get; set; }
        public string _iconSrc { get; set; }
        public string _timeSpan { get; set; }
        public int _limit { get; set; }

        public string curr;
        public string _priceCurr {
            get { return curr; }
            set {
                curr = value;
                RaiseProperty(nameof(_priceCurr));
            }
        }
        private string diff;
        public string _priceDiff {
            get { return diff; }
            set {
                if(value != diff) {
                    diff = value;
                    if (value.StartsWith("▼"))
                        _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                    else
                        _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];

                    RaiseProperty(nameof(_priceDiff));
                }
            }
        }

        private string vol24;
        public string _volume24 {
            get { return vol24; }
            set { if (value != vol24) {
                    vol24 = value;
                    RaiseProperty(nameof(_volume24));
            } }
        }

        private string vol24to;
        public string _volume24to {
            get { return vol24to; }
            set { if (value != vol24to) {
                    vol24to = value;
                    RaiseProperty(nameof(_volume24to));
            } }
        }

        private SolidColorBrush fg;
        public SolidColorBrush _priceDiffFG {
            get { return fg; }
            set {
                fg = value;
                RaiseProperty(nameof(_priceDiffFG));
            }
        }
    }
}

