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
    public sealed partial class Page_LTC : Page {

        private static int limit = 60;
        private static string timeSpan = "day";

        public Page_LTC() {
            this.InitializeComponent();
            InitValues();

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour")
                        LTC_TimerangeButton_Click(r, null);
                });
            }, period);

        }

        async private void InitValues() {
            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                LTC_TimerangeButton_Click(r, null);

            } catch (Exception) {
                LoadingControl.IsLoading = false;
                LTC_curr.Text = "Error!";
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For Sync button //////////////////////////////////////////////////////////////////////////////////
        internal async void UpdatePage() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;

            await UpdateLTC();
            LTC_verticalAxis.Minimum = getMinimum(App.ppLTC);
            LTC_verticalAxis.Maximum = getMaximum(App.ppLTC);
            LTC_DateTimeAxis = App.AdjustAxis(LTC_DateTimeAxis, timeSpan);
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

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject { Date   = App.ppLTC[i].DateTime,
                                                            Value  = (App.ppLTC[i].Low + App.ppLTC[i].High) / 2,
                                                            Low    = App.ppLTC[i].Low,
                                                            High   = App.ppLTC[i].High,
                                                            Open   = App.ppLTC[i].Open,
                                                            Close  = App.ppLTC[i].Close,
                                                            Volume = App.ppLTC[i].Volumefrom
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

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        async private Task GetStats() {

            await App.GetStats("LTC");

            LTC_Open.Text  = App.stats.Open24 + App.coinSymbol;
            LTC_High.Text  = App.stats.High24 + App.coinSymbol;
            LTC_Low.Text   = App.stats.Low24  + App.coinSymbol;
            LTC_Vol24.Text = App.stats.Volume24 + "LTC";
        }

        async private Task Get24Volume() {
            await App.GetHisto("LTC", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add( new App.ChartDataObject() {
                    Date = App.ppLTC[i].DateTime,
                    Volume = App.ppLTC[i].Volumefrom
                });
            }
            this.VolumeChart.DataContext = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
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
