﻿using CryptoTracker.Helpers;
using CryptoTracker.Model;
using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
    public sealed partial class Home : Page {

        static ObservableCollection<HomeTile> homeCoinList { get; set; }
        private static string diff = "0";
        private static int limit = 1500;
        private static string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            homeCoinList = new ObservableCollection<HomeTile>();
            PriceListView.ItemsSource = homeCoinList;
            VolumeListView.ItemsSource = homeCoinList;

            InitHome();

            homeCoinList.CollectionChanged += HomeCoinList_CollectionChanged;
        }

        private void HomeCoinList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            EmptyPageWarning.Visibility = (((Collection<HomeTile>)sender).Count > 0) ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void InitHome() {
            // First keep an updated list of coins
            await App.GetCoinList();

            // Then update home-coin-tiles
            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                try {
                    await AddCoinHome(App.pinnedCoins[i]);
                    await UpdateCard(i);
                } catch {
                    
                }
            }

            
        }

        // #########################################################################################
        // Add/remove coins from Home
        internal static async Task AddCoinHome(string crypto) {

            if (crypto == "MIOTA")
                crypto = "IOT";

            String iconPath = "/Assets/Icons/icon" + crypto + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch (Exception) {
                iconPath = "/Assets/Icons/iconNULL.png";
            }

            homeCoinList.Add(new HomeTile {
                _cryptoName = crypto,
                _priceDiff = diff,
                _crypto = crypto,
                _iconSrc = iconPath,
                _timeSpan = timeSpan,
                _limit = limit,
                _opacity = 1,
            });

            // Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        internal static void RemoveCoinHome(string crypto) {
            if (App.pinnedCoins.Contains(crypto)) {
                var n = App.pinnedCoins.IndexOf(crypto);

                App.pinnedCoins.RemoveAt(n);
                homeCoinList.RemoveAt(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }

        // #########################################################################################
        //  Update all cards
        internal async Task UpdateAllCards() {            
            foreach (HomeTile homeTile in homeCoinList) {
                homeTile._opacity = 0.33;
            }
            for (int i = 0; i < homeCoinList.Count; i++) {
                await UpdateCard(i);
                homeCoinList[i]._opacity = 1;
            }
            
        }

        private async Task UpdateCard(int i) {
            
            string c = App.pinnedCoins[i];

			try {
                // DATA:
                await App.GetHisto(c, timeSpan, limit);

                float oldestPrice;
                float newestPrice;
                if (App.historic != null) {
                    oldestPrice = App.historic[0].Close;
                    newestPrice = App.historic[App.historic.Count - 1].Close;
                }
                else {
                    oldestPrice = 0;
                    newestPrice = 0;
                }

                float d = (float)Math.Round(((newestPrice / oldestPrice) - 1) * 100, 2);

                if (d < 0) {
                    d = Math.Abs(d);
                    diff = "▼" + d.ToString() + "%";
                }
                else
                    diff = "▲" + d.ToString() + "%";

                homeCoinList[i]._priceCurr = App.GetCurrentPrice(c, "defaultMarket").ToString() + App.coinSymbol;
                homeCoinList[i]._priceDiff = diff;

                await App.GetCoinStats(c, "defaultMarket");
                homeCoinList[i]._volume24 = App.stats.Volume24;
                homeCoinList[i]._volume24to = App.stats.Volume24To;

                // #########################################################################################
                // LOADING BAR
                ListViewItem container = (ListViewItem)PriceListView.ContainerFromIndex(i);
                if (container == null)
                    return;
                var loading = (container.ContentTemplateRoot as FrameworkElement)?.FindName("LoadingControl") as Loading;
                loading.IsLoading = true;

                // #########################################################################################
                // COLOR
                SolidColorBrush coinColorT, coinColor;
                try {
                    coinColorT = (SolidColorBrush)Application.Current.Resources[c.ToUpper() + "_colorT"];
                    coinColor = (SolidColorBrush)Application.Current.Resources[c.ToUpper() + "_color"];
                }
                catch (Exception) {
                    coinColorT = (SolidColorBrush)Application.Current.Resources["Main_WhiteBlackT"];
                    coinColor = (SolidColorBrush)Application.Current.Resources["Main_WhiteBlack"];
                }

                // #########################################################################################
                // PRICE CHART

                var PriceChart = (container.ContentTemplateRoot as FrameworkElement)?.FindName("PriceChart") as RadCartesianChart;
                var verticalAxis = (container.ContentTemplateRoot as FrameworkElement)?.FindName("VerticalAxis") as LinearAxis;

                await App.GetHisto(c, timeSpan, limit);
                List<ChartData> priceData = new List<ChartData>();
                verticalAxis.Minimum = GraphHelper.GetMinimum(App.historic);
                verticalAxis.Maximum = GraphHelper.GetMaximum(App.historic);

                for (int k = 0; k < App.historic.Count; ++k) {
                    priceData.Add(new ChartData() {
                        Date = App.historic[k].DateTime,
                        Value = (App.historic[k].Low + App.historic[k].High) / 2,
                        Low = App.historic[k].Low,
                        High = App.historic[k].High,
                        Open = App.historic[k].Open,
                        Close = App.historic[k].Close,
                        Volume = App.historic[k].Volumefrom
                    });
                }

                SplineAreaSeries series = (SplineAreaSeries)PriceChart.Series[0];
                series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
                series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
                series.ItemsSource = priceData;
                series.Fill = coinColorT;
                series.Stroke = coinColor;
                var v = series.VerticalAxis;

                // #########################################################################################
                // VOLUME CHART
                ListViewItem container2 = (ListViewItem)VolumeListView.ContainerFromIndex(i);
                await App.GetHisto(c, "hour", 24);

                List<ChartData> volumeData = new List<ChartData>();
                for (int j = 0; j < 24; j++) {
                    volumeData.Add(new ChartData() {
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
            } catch (Exception e) {
                Analytics.TrackEvent("UNHANDLED2: " + e.Message);
            }
            
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

            UpdateAllCards();
        }

        private void homeListView_Click(object sender, ItemClickEventArgs e) {
            // Connected animation
            switch ( ((ListView)sender).Name ) {
                case "PriceListView":
                    PriceListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "PriceListView_Element");
                    break;

                case "VolumeListView":
                    VolumeListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "VolumeListView_Element");
                    break;
            }

            var clickedItem = (HomeTile)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem._crypto);
        }

        private void UnpinCoin(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext)._crypto;

            RemoveCoinHome(crypto);
        }
        private void MoveCoinDown(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext)._crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if(n < homeCoinList.Count - 1) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n + 1];
                App.pinnedCoins[n + 1] = tempName;

                var tempListItem = homeCoinList[n];
                homeCoinList[n] = homeCoinList[n + 1];
                homeCoinList[n + 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
        private void MoveCoinUp(object sender, RoutedEventArgs e) {
            string crypto = ((HomeTile)((FrameworkElement)sender).DataContext)._crypto;
            int n = App.pinnedCoins.IndexOf(crypto);

            if (n != 0) {
                var tempName = App.pinnedCoins[n];
                App.pinnedCoins[n] = App.pinnedCoins[n - 1];
                App.pinnedCoins[n - 1] = tempName;

                var tempListItem = homeCoinList[n];
                homeCoinList[n] = homeCoinList[n - 1];
                homeCoinList[n - 1] = tempListItem;

                PriceListView.UpdateLayout();
                UpdateCard(n);

                // Update pinnedCoin list
                App.UpdatePinnedCoins();
            }
        }
    }
}

