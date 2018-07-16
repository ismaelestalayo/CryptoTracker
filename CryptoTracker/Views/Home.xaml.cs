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

namespace CryptoTracker.Views {
    public sealed partial class Home : Page {

        static ObservableCollection<HomeTileClass> homeCoinList { get; set; }
        private string diff = "0";
        private int limit = 60;
        private string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();

            homeCoinList = new ObservableCollection<HomeTileClass>();
            homeListView.ItemsSource = homeCoinList;

            InitHome();
            // keep an updated list of coins
            App.GetCoinList();
        }

        private async void InitHome() {

            Frame contentFrame = Window.Current.Content as Frame;
            if (contentFrame.Content != null) {
                MainPage mp = contentFrame.Content as MainPage;
                TextBlock t = mp.FindName("mainTitle") as TextBlock;
                t.Text = "Dashboard";
            }

            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                await AddCoinHome(App.pinnedCoins[i]);
            }

            await UpdateAllCards();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task AddCoinHome(string c) {

            String iconPath = "/Assets/icon" + c + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch(Exception) {
                iconPath = "/Assets/iconNULL.png";
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
            //for (int j = 0; j < homeCoinList.Count; j++) {
            //    ListViewItem cc = (ListViewItem)homeListView.ContainerFromItem(homeListView.Items[j]);
            //    var loadingg = (cc.ContentTemplateRoot as FrameworkElement)?.FindName("LoadingControl") as Loading;
            //    loadingg.IsLoading = true;
            //}

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

                

                // LOADING BAR:
                var item = homeListView.Items[i];
                ListViewItem container = (ListViewItem)homeListView.ContainerFromItem(item);
                var loading = (container.ContentTemplateRoot as FrameworkElement)?.FindName("LoadingControl") as Loading;
                loading.IsLoading = true;
                
                // CHARTS:
                if (container == null)
                    break;

                var temp = (container.ContentTemplateRoot as FrameworkElement)?.FindName("radChart") as FrameworkElement;
                RadCartesianChart radChart = (RadCartesianChart)temp;

                c = App.pinnedCoins[i];
                await App.GetHisto(c, timeSpan, limit);
                List<App.ChartDataObject> data = new List<App.ChartDataObject>();

                for (int k = 0; k < App.historic.Count; ++k) {
                    App.ChartDataObject obj = new App.ChartDataObject {
                        Date    = App.historic[k].DateTime,
                        Value   =(App.historic[k].Low + App.historic[k].High) / 2,
                        Low     = App.historic[k].Low,
                        High    = App.historic[k].High,
                        Open    = App.historic[k].Open,
                        Close   = App.historic[k].Close,
                        Volume  = App.historic[k].Volumefrom
                    };
                    data.Add(obj);
                }

                SplineAreaSeries series = (SplineAreaSeries)radChart.Series[0];
                series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
                series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
                series.ItemsSource = data;

                try {
                    series.Fill     = (SolidColorBrush)Application.Current.Resources[c + "_colorT"];
                    series.Stroke   = (SolidColorBrush)Application.Current.Resources[c + "_color"];
                } catch (Exception) {
                    series.Fill     = (SolidColorBrush)Application.Current.Resources["null_colorT"];
                    series.Stroke   = (SolidColorBrush)Application.Current.Resources["null_color"];
                }

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

