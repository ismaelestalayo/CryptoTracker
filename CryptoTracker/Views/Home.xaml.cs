using CryptoTracker.Helpers;
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
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI;

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
            // keep an updated list of coins
            App.GetCoinList();
        }

        private async void InitHome() {
            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                await AddCoinHome(App.pinnedCoins[i]);
            }

            await UpdateAllCards();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        internal async Task UpdateAllCards() {

            for (int i = 0; i < homeCoinList.Count; i++) {
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
                homeCoinList[i]._volume24   = App.stats.Volume24;
                homeCoinList[i]._volume24to = App.stats.Volume24To;
                //statsOpen.Text = App.stats.Open24;
                //statsHigh.Text = App.stats.High24;
                //statsLow.Text = App.stats.Low24;
                //statsVol24.Text = App.stats.Volume24;
                //supply.Text = App.stats.Supply;
                //marketcap.Text = App.stats.Marketcap;
                //totVol24.Text = "Total Vol 24h: " + App.stats.Volume24;

                // LOADING BAR:
                ListViewItem container = (ListViewItem)priceListView.ContainerFromIndex(i);
                var loading = (container.ContentTemplateRoot as FrameworkElement)?.FindName("LoadingControl") as Loading;
                loading.IsLoading = true;


                // COLOR:
                SolidColorBrush coinColorT, coinColor;
                try {
                    coinColorT = (SolidColorBrush)Application.Current.Resources[c.ToUpper() + "_colorT"];
                    coinColor = (SolidColorBrush)Application.Current.Resources[ c.ToUpper() + "_color"];
                } catch (Exception) {
                    coinColorT = (SolidColorBrush)Application.Current.Resources["null_colorT"];
                    coinColor = (SolidColorBrush)Application.Current.Resources["null_color"];
                }

                // PRICE CHART:
                if (container == null)
                    break;

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


                // VOLUME CHART:
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
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
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
            UpdateAllCards();
        }

        private void homeListView_Click(object sender, ItemClickEventArgs e) {
            var clickedItem = (HomeTileClass)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem._crypto);
        }
    }



    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    internal class HomeTileClass : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string _cryptoName { get; set; }
        public string _crypto { get; set; }
        public string _iconSrc { get; set; }
        public string _timeSpan { get; set; }
        public string _volume24 { get; set; }
        public string _volume24to { get; set; }
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
                diff = value;
                if (value.StartsWith("▼"))
                    _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelRed"];
                else
                    _priceDiffFG = (SolidColorBrush)App.Current.Resources["pastelGreen"];
                RaiseProperty(nameof(_priceDiff));
            }
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

