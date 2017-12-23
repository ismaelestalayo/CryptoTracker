using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {
    public sealed partial class Page_Home : Page {

        private int limit = 60;
        private string timeSpan = "hour";

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_Home() {
            this.InitializeComponent();

            UpdateHome();

            //TimeSpan period = TimeSpan.FromSeconds(60);
            //ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => {
            //    Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
            //        RadioButton r = new RadioButton { Content = timeSpan };
            //        if (timeSpan == "hour")
            //            ALL_TimerangeButton_Click(r, null);
            //    });
            //}, period);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal async void UpdateHome() {

            if (LoadingControl_BTC == null)
                LoadingControl_BTC = new Loading();
            if (LoadingControl_ETH == null)
                LoadingControl_ETH = new Loading();
            if (LoadingControl_LTC == null)
                LoadingControl_LTC = new Loading();

            LoadingControl_BTC.IsLoading = true;
            LoadingControl_ETH.IsLoading = true;
            LoadingControl_LTC.IsLoading = true;

            await UpdateBTC();
            BTC_verticalAxis.Minimum = getMinimum(App.ppBTC);
            BTC_verticalAxis.Maximum = getMaximum(App.ppBTC);
            BTC_DateTimeAxis = App.AdjustAxis(BTC_DateTimeAxis, timeSpan);
            await GetStats("BTC");
            await Get24hVolume("BTC");

            await UpdateETH();
            ETH_verticalAxis.Minimum = getMinimum(App.historic);
            ETH_verticalAxis.Maximum = getMaximum(App.historic);
            ETH_DateTimeAxis = App.AdjustAxis(ETH_DateTimeAxis, timeSpan);
            await GetStats("ETH");
            await Get24hVolume("ETH");

            await UpdateLTC();
            LTC_verticalAxis.Minimum = getMinimum(App.historic);
            LTC_verticalAxis.Maximum = getMaximum(App.historic);
            LTC_DateTimeAxis = App.AdjustAxis(LTC_DateTimeAxis, timeSpan);
            await GetStats("LTC");
            await Get24hVolume("LTC");
        }

        private float getMaximum(List<App.PricePoint> a) {
            int i = 0;
            float max = 0;

            foreach (App.PricePoint type in a) {
                if (a[i].High > max)
                    max = a[i].High;

                i++;
            }

            return max;
        }
        private float getMinimum(List<App.PricePoint> a) {
            int i = 0;
            float min = 15000;

            foreach (App.PricePoint type in a) {
                if (a[i].High < min)
                    min = a[i].High;

                i++;
            }

            return min * (float)0.993;
        }

        async public Task GetStats(String crypto) {

            await App.GetStats(crypto);

            switch (crypto) {
                case "BTC":
                    BTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    BTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    BTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    BTC_Vol24.Text = App.stats.Volume24 + "BTC";
                    break;

                case "ETH":
                    ETH_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    ETH_High.Text  = App.stats.High24   + App.coinSymbol;
                    ETH_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    ETH_Vol24.Text = App.stats.Volume24 + "ETH";
                    break;

                case "LTC":
                    LTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    LTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    LTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    LTC_Vol24.Text = App.stats.Volume24 + "LTC";
                    break;

            }
        }
        async private Task Get24hVolume(String crypto) {
            await App.GetHisto(crypto, "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();

            switch (crypto) {
                case "BTC":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.ppBTC[i].DateTime,
                            Volume = App.ppBTC[i].Volumefrom
                        });
                    }
                    this.BTC_VolumeChart.DataContext = data;
                    break;

                case "ETH":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.historic[i].DateTime,
                            Volume = App.historic[i].Volumefrom
                        });
                    }
                    this.ETH_VolumeChart.DataContext = data;
                    break;

                case "LTC":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.historic[i].DateTime,
                            Volume = App.historic[i].Volumefrom
                        });
                    }
                    this.LTC_VolumeChart.DataContext = data;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = App.BTC_now.ToString() + App.coinSymbol;

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("BTC", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("BTC", "hour", limit);
                    break;

                case "year":
                    await App.GetHisto("BTC", "day", limit);
                    break;

                case "all":
                    await App.GetHisto("BTC", "day", 0);
                    break;
            }

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject {
                    Date = App.ppBTC[i].DateTime,
                    Value = App.ppBTC[i].Low
                };
                data.Add(obj);
            }

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            if (timeSpan.Equals("hour"))
                App.BTC_change1h = dBTC;

            if (dBTC < 0) { 
                BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                dBTC = Math.Abs(dBTC);
                BTC_diff.Text = "▼" + dBTC.ToString() + "%";
            } else {
                BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                BTC_diff.Text = "▲" + dBTC.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_BTC.IsLoading = false;
        }
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = App.ETH_now.ToString() + App.coinSymbol;

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("ETH", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("ETH", "hour", limit);
                    break;

                case "year":
                    await App.GetHisto("ETH", "day", limit);
                    break;

                case "all":
                    await App.GetHisto("ETH", "day", 0);
                    break;
            }

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject {
                    Date  = App.historic[i].DateTime,
                    Value = App.historic[i].Low };
                data.Add(obj);
            }

            float dETH = ((App.ETH_now / App.ETH_old) - 1) * 100;
            dETH = (float)Math.Round(dETH, 2);
            if (timeSpan.Equals("hour"))
                App.ETH_change1h = dETH;

            if (dETH < 0) {
                ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                dETH = Math.Abs(dETH);
                ETH_diff.Text = "▼" + dETH.ToString() + "%";
            } else {
                ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                ETH_diff.Text = "▲" + dETH.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_ETH.IsLoading = false;
        }
        async public Task UpdateLTC() {
            await App.GetCurrentPrice("LTC");
            LTC_curr.Text = App.LTC_now.ToString() + App.coinSymbol;

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("LTC", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("LTC", "hour", limit);
                    break;

                case "year":
                    await App.GetHisto("LTC", "day", limit);
                    break;

                case "all":
                    await App.GetHisto("LTC", "day", 0);
                    break;
            }

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject {
                    Date  = App.historic[i].DateTime,
                    Value = App.historic[i].Low
                };
                data.Add(obj);
            }

            float dLTC = ((App.LTC_now / App.LTC_old) - 1) * 100;
            dLTC = (float)Math.Round(dLTC, 2);
            if (timeSpan.Equals("hour"))
                App.LTC_change1h = dLTC;

            if (dLTC < 0) {
                LTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                dLTC = Math.Abs(dLTC);
                LTC_diff.Text = "▼" + dLTC.ToString() + "%";
            } else {
                LTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                LTC_diff.Text = "▲" + dLTC.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_LTC.IsLoading = false;
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
            UpdateHome();
        }

        private void BTC_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            this.Frame.Navigate(typeof(Page_BTC));
        }
        private void ETH_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            this.Frame.Navigate(typeof(Page_CoinTemplate), "ETH");
        }
        private void LTC_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e) {
            this.Frame.Navigate(typeof(Page_CoinTemplate), "LTC");
        }

    }
}
