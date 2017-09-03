using CoinBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_BTC : Page {

        internal int limit = 60;
        private string timeSpan = "day";

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_BTC() {
            this.InitializeComponent();
            InitValues();
        }

        async private void InitValues() {
            try {
                await UpdateBTC();
                await GetStats();

            } catch (Exception ex) {
                BTC_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateBTC();
            BTC_slider_changed(BTC_slider, null);
            GetStats();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = "Current price: " + App.BTC_now.ToString();
            if (App.coin.Equals("EUR")) {
                BTC_curr.Text += "€";
            } else{
                BTC_curr.Text += "$";
            }

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
                ChartDataObject obj = new ChartDataObject { Date = App.ppBTC[i].DateTime, Value = App.ppBTC[i].Low };
                data.Add(obj);
            }

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            BTC_diff.Text = dBTC.ToString() + "%";
            if (dBTC < 0) {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
            }
            else {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
            }

            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.Name = "Bitcoin";
            series.ItemsSource = data;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    BTC_from.Text = "Last hour: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    BTC_DateTimeAxis.MajorStep = 10;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case 2:
                    BTC_from.Text = "Last day: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    BTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case 3:
                    BTC_from.Text = "Last week: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case 4:
                    BTC_from.Text = "Last month: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;

                case 5:
                    BTC_from.Text = "Last year: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case 6:
                    BTC_from.Text = "Sorry, can't go back in time so far ";
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
