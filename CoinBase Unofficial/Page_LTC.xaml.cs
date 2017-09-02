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
    public sealed partial class Page_LTC : Page {

        internal int limit = 60;
        private string timeSpan = "hour";

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_LTC() {
            this.InitializeComponent();
            InitValues();

        }

        async private void InitValues() {
            try {
                await UpdateLTC();
            } catch (Exception ex) {
                LTC_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void LTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateLTC();
            LTC_slider_changed(LTC_slider, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateLTC() {
            await App.GetData("LTC");
            LTC_curr.Text = "Current price: " + App.LTC_now.ToString();
            if (App.coin.Equals("EUR"))
                LTC_curr.Text += "€";
            else {
                LTC_curr.Text += "$";
            }

            switch (timeSpan) {
                case "hour":
                    await App.GetHisto("LTC", "minute", limit);
                    break;
                case "day":
                    await App.GetHisto("LTC", "minute", limit);
                    break;
                case "week":
                    await App.GetHisto("LTC", "hour", limit);
                    break;
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
                ChartDataObject obj = new ChartDataObject { Date = App.ppLTC[i].DateTime, Value = App.ppLTC[i].Low };
                data.Add(obj);
            }

            float dLTC = ((App.LTC_now / App.LTC_old) - 1) * 100;
            dLTC = (float)Math.Round(dLTC, 2);
            LTC_diff.Text = dLTC.ToString() + "%";
            if (dLTC < 0) {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
            }
            else {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
            }

            AreaSeries series = (AreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LTC_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    LTC_from.Text = "Last hour: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    LTC_DateTimeAxis.MajorStep = 10;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;
                case 2:
                    LTC_from.Text = "Last day: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    LTC_DateTimeAxis.MajorStep = 6;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case 3:
                    LTC_from.Text = "Last week: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case 4:
                    LTC_from.Text = "Last month: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;

                case 5:
                    LTC_from.Text = "Last year: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "all";
                    limit = 0;
                    break;

                case 6:
                    LTC_from.Text = "Can't go back in time so far ";
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
