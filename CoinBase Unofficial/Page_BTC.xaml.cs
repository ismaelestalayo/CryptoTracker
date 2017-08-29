using CoinBase;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CoinBase {
    public sealed partial class Page_BTC : Page {

        internal static int granularityBTC = 3600;
        internal static int numBTC = 60;


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

            } catch (Exception ex) {
                BTC_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateBTC();
            BTC_slider_changed(BTC_slider, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetData("BTC-EUR");
            BTC_curr.Text = "Current price: " + App.currency_BTC.ToString() + "€";

            await App.GetHistoricValues(granularityBTC, "BTC-EUR");

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < numBTC; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppBTC[i].DateTime, Value = App.ppBTC[i].Low };
                data.Add(obj);
            }

            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    BTC_diff.Text = "Last hour: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    BTC_DateTimeAxis.MajorStep = 10;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    granularityBTC = 60;
                    numBTC = 60;
                    break;
                case 2:
                    BTC_diff.Text = "Last day: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    BTC_DateTimeAxis.MajorStep = 6;
                    granularityBTC = 900;
                    numBTC = 100;
                    break;
                case 3:
                    BTC_diff.Text = "Last week: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    granularityBTC = 3600;
                    numBTC = 200;
                    break;
                case 4:
                    BTC_diff.Text = "Last month: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    granularityBTC = 14400;
                    numBTC = 250;
                    break;
                case 5:
                    BTC_diff.Text = "Last year: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    granularityBTC = 14400;
                    numBTC = 401;
                    break;
                case 6:
                    BTC_diff.Text = "Sorry, can't go back in time so far ";
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    granularityBTC = 14400;
                    numBTC = 401;
                    break;
            }

            UpdateBTC();
        }
    }
}
