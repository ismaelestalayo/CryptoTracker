using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_LTC : Page {

        internal static int limit = 60;
        internal static string timeSpan = "day";

        public Page_LTC() {
            this.InitializeComponent();
            InitValues();
        }

        async private Task InitValues() {
            try {
                await LTC_Update_click(null, null);

            } catch (Exception ex) {
                LoadingControl.IsLoading = false;
                LTC_curr.Text = "Error: " + ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public async Task LTC_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = timeSpan };
            LTC_TimerangeButton_Click(r, null);
            await GetStats();
            Get24Volume();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void UpdateLTC() {
            App.GetCurrentPrice("LTC");
            LTC_curr.Text = App.LTC_now.ToString();
            LTC_curr.Text = (App.coin.Equals("EUR")) ? LTC_curr.Text += "€" : LTC_curr.Text += "$";

            switch (timeSpan) {
                case "hour":
                case "day":
                    App.GetHisto("LTC", "minute", limit);
                    break;

                case "week":
                case "month":
                    App.GetHisto("LTC", "hour", limit);
                    break;

                case "year":
                    App.GetHisto("LTC", "day", limit);
                    break;

                case "all":
                    App.GetHisto("LTC", "day", 0);
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
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dLTC = Math.Abs(dLTC);
                LTC_diff.Text = "▼";
            } else {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                LTC_diff.Text = "▲";
            }
            LTC_diff.Text += dLTC.ToString() + "%";

            AreaSeries series = (AreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl.IsLoading = false;
        }

        async private Task GetStats() {

            await App.GetStats("LTC");

            string sym;
            if (App.coin.Equals("EUR")) {
                sym = "€";
            } else {
                sym = "$";
            }

            LTC_Open.Text  = App.stats.Open24 + sym;
            LTC_High.Text  = App.stats.High24 + sym;
            LTC_Low.Text   = App.stats.Low24 + sym;
            LTC_Vol24.Text = App.stats.Volume24 + "LTC";
        }
        private void Get24Volume() {
            App.GetHisto("LTC", "hour", 24);

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
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    LTC_DateTimeAxis.MajorStep = 10;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    LTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    LTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    LTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateLTC();
        }
    }
}
