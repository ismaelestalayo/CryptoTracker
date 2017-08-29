using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CoinBase {
    public sealed partial class Page_LTC : Page {
        
        internal static int granularityLTC = 3600;
        internal static int numLTC = 60;

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
            await App.GetData("LTC-EUR");
            LTC_curr.Text = "Current price: " + App.currency_LTC.ToString() + "€";

            await App.GetHistoricValues(granularityLTC, "LTC-USD");

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < numLTC; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppLTC[i].DateTime, Value = App.ppLTC[i].Low };
                data.Add(obj);
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
                    LTC_diff.Text = "Last hour: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    LTC_DateTimeAxis.MajorStep = 10;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    granularityLTC = 60;
                    numLTC = 60;
                    break;
                case 2:
                    LTC_diff.Text = "Last day: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    LTC_DateTimeAxis.MajorStep = 6;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    granularityLTC = 900;
                    numLTC = 100;
                    break;
                case 3:
                    LTC_diff.Text = "Last week: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    granularityLTC = 3600;
                    numLTC = 200;
                    break;
                case 4:
                    LTC_diff.Text = "Last month: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    granularityLTC = 14400;
                    numLTC = 250;
                    break;
                case 5:
                    LTC_diff.Text = "Last year: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    granularityLTC = 14400;
                    numLTC = 401;
                    break;
                case 6:
                    LTC_diff.Text = "Can't go back in time so far ";
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    granularityLTC = 14400;
                    numLTC = 401;
                    break;
            }

            UpdateLTC();
        }



    }
}
