using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_BTC : Page {

        private static int limit = 60;
        private static string timeSpan = "day";

        public Page_BTC() {
            this.InitializeComponent();
            InitValues();

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour")
                        BTC_TimerangeButton_Click(r, null);
                });
            }, period);

        }

        async private void InitValues() {
            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                BTC_TimerangeButton_Click(r, null);

            } catch (Exception) {
                LoadingControl.IsLoading = false;
                BTC_curr.Text = "Error!";
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For Sync button //////////////////////////////////////////////////////////////////////////////////
        internal async void UpdatePage() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;

            await UpdateBTC();
            BTC_verticalAxis.Minimum = getMinimum(App.ppBTC);
            BTC_verticalAxis.Maximum = getMaximum(App.ppBTC);
            BTC_DateTimeAxis = App.AdjustAxis(BTC_DateTimeAxis, timeSpan);
            await GetStats();
            await Get24Volume();
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
            return min * (float)0.99;
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

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject {
                    Date = App.ppBTC[i].DateTime,
                    Value = (App.ppBTC[i].Low + App.ppBTC[i].High) / 2,
                    Low = App.ppBTC[i].Low,
                    High = App.ppBTC[i].High,
                    Open = App.ppBTC[i].Open,
                    Close = App.ppBTC[i].Close,
                    Volume = App.ppBTC[i].Volumefrom
                };
                data.Add(obj);

            }

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            if (timeSpan.Equals("hour"))
                App.BTC_change1h = dBTC;

            if (dBTC < 0) {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dBTC = Math.Abs(dBTC);
                BTC_diff.Text = "▼" + dBTC.ToString() + "%";
            } else {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                BTC_diff.Text = "▲" + dBTC.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        async public Task GetStats() {

            await App.GetStats("BTC");

            BTC_Open.Text = App.stats.Open24 + App.coinSymbol;
            BTC_High.Text = App.stats.High24 + App.coinSymbol;
            BTC_Low.Text = App.stats.Low24 + App.coinSymbol;
            BTC_Vol24.Text = App.stats.Volume24 + "BTC";
        }
        async private Task Get24Volume() {
            await App.GetHisto("BTC", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add(new App.ChartDataObject() {
                    Date = App.ppBTC[i].DateTime,
                    Volume = App.ppBTC[i].Volumefrom
                });
            }
            this.VolumeChart.DataContext = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl.IsLoading = true;
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
            UpdatePage();
        }
    }
}
