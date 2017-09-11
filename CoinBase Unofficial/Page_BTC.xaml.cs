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
    public sealed partial class Page_BTC : Page {

        internal static int limit = 60;
        internal static string timeSpan = "day";

        public Page_BTC() {
            this.InitializeComponent();
            InitValues();
        }

        async private void InitValues() {
            try {
                BTC_Update_click(null, null);
                await GetStats();
                await Get24Volume();

            } catch (Exception ex) {
                LoadingControl.IsLoading = false;
                BTC_curr.Text = "Error: " + ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For SyncAll button
        public void BTC_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = timeSpan };
            BTC_TimerangeButton_Click(r, null);
            GetStats();
            Get24Volume();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = App.BTC_now.ToString();
            BTC_curr.Text = (App.coin.Equals("EUR")) ? BTC_curr.Text += "€" : BTC_curr.Text += "$";

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
                App.ChartDataObject obj = new App.ChartDataObject { Date   = App.ppBTC[i].DateTime,
                                                            Value  =(App.ppBTC[i].Low + App.ppBTC[i].High) / 2,
                                                            Low    = App.ppBTC[i].Low,
                                                            High   = App.ppBTC[i].High,
                                                            Open   = App.ppBTC[i].Open,
                                                            Close  = App.ppBTC[i].Close,
                                                            Volume = App.ppBTC[i].Volumefrom
                };
                data.Add(obj);

            }

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            if (dBTC < 0) {
                BTC_diff.Foreground = BTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                BTC_difff.Text = "\xEB0F"; //C# parser works different from XAML parser
                dBTC = Math.Abs(dBTC);
            } else {
                BTC_diff.Foreground = BTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                BTC_difff.Text = "\xEB11";
            }
            BTC_diff.Text = dBTC.ToString() + "%";

            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl.IsLoading = false;
        }

        async public Task GetStats() {

            await App.GetStats("BTC");

            string sym;
            if (App.coin.Equals("EUR")) {
                sym = "€";
            } else {
                sym = "$";
            }

            BTC_Open.Text  = App.stats.Open24 + sym;
            BTC_High.Text  = App.stats.High24 + sym;
            BTC_Low.Text   = App.stats.Low24 + sym;
            BTC_Vol24.Text = App.stats.Volume24 + "BTC";
        }
        async private Task Get24Volume() {
            await App.GetHisto("BTC", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add( new App.ChartDataObject() {
                    Date = App.ppBTC[i].DateTime,
                    Volume = App.ppBTC[i].Volumefrom
                });
            }
            this.VolumeChart.DataContext = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl.IsLoading = true;
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    BTC_DateTimeAxis.MajorStep = 10;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    BTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    BTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    BTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateBTC();
        }
    }
}
