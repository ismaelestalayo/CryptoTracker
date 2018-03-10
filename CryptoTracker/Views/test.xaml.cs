using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
    public sealed partial class test : Page {

        static ObservableCollection<homeCoinsClass> homeCoinList { get; set; }
        private string diff;
        public string crypto = "BTC";

        public test() {
            this.InitializeComponent();

            homeCoinList = new ObservableCollection<homeCoinsClass>();


            for (int i = 0; i < App.pinnedCoins.Count; i++) {
                updateChartAsync(App.pinnedCoins[i] ).ConfigureAwait(true);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task updateChartAsync(string c) {
            await App.GetHisto(c, "minute", 60);

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

            homeCoinList.Add(new homeCoinsClass {
                _cryptoName = c,
                _priceCurr = App.GetCurrentPrice(c, "defaultMarket").ToString() + App.coinSymbol,
                _priceDiff = diff,
            });

            homeListView.ItemsSource = homeCoinList;
        }


    }

}

