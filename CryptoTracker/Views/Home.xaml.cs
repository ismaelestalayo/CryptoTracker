﻿using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {
    public sealed partial class Home : Page {

        static ObservableCollection<HomeTileClass> homeCoinList { get; set; }
        private string diff;
        private int limit = 60;
        private string timeSpan = "minute";

        public Home() {
            this.InitializeComponent();
            
            homeCoinList = new ObservableCollection<HomeTileClass>();
            homeListView.ItemsSource = homeCoinList;
            uuuuuuupdate();
        }

        private async void uuuuuuupdate() {
            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                await updateChart(App.pinnedCoins[i]);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task updateChart(string c) {
            await App.GetHisto(c, timeSpan, limit);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < App.historic.Count; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject {
                    Date = App.historic[i].DateTime,
                    Value = (App.historic[i].Low + App.historic[i].High) / 2,
                };
                data.Add(obj);
            }

            float d = 0;
            float oldestPrice = App.historic[0].Close;
            float newestPrice = App.historic[App.historic.Count - 1].Close;
            d = (float)Math.Round(((newestPrice / oldestPrice) - 1) * 100, 2);

            if (d < 0) {
            //    BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                d = Math.Abs(d);
                diff = "▼" + d.ToString() + "%";
            } else {
            //    BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                diff = "▲" + d.ToString() + "%";
            }
            
            String iconPath = "/Assets/icon" + c + ".png";
            try {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx://" + iconPath));
            } catch(Exception ex) {
                iconPath = "/Assets/iconNULL.png";
            }

            homeCoinList.Add(new HomeTileClass {
                _cryptoName = c,
                _priceCurr = App.GetCurrentPrice(c, "defaultMarket").ToString() + App.coinSymbol,
                _priceDiff = diff,
                _crypto = c,
                _iconSrc = iconPath
            });

            //homeListView.ItemsSource = homeCoinList;
        }

        private void testHome() {
            for (int i = 0;  i< homeListView.Items.Count; i++) {
                homeCoinList[i]._crypto = "bobo";
                homeCoinList[i]._cryptoName = "bobo";
                homeCoinList[i]._priceCurr = "bobo";
            }
            homeListView.ItemsSource = homeCoinList;
        }

        private void ALL_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            testHome();
        }

        private void homeListView_Click(object sender, ItemClickEventArgs e) {
            var clickedItem = (HomeTileClass)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem._crypto);
        }
    }

}

