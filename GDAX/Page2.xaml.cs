using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace GDAX {
    public sealed partial class Page2 : Page {

        internal static int granularityBTC = 3600;
        internal static int granularityETH = 3600;
        internal static int granularityLTC = 3600;
        internal static int numBTC = 60;
        internal static int numETH = 60;
        internal static int numLTC = 60;

        internal static int loop = 0;

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page2() {
            this.InitializeComponent();
            InitValues();

        }

        async private void InitValues() {
            try {
                await UpdateBTC();
                await UpdateETH();
                await UpdateLTC();
            } catch (Exception ex) {
                ETH_curr.Text = "Maybe you have no internet?";
                BTC_curr.Text = "Maybe you have no internet?";
                LTC_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateBTC();
            BTC_slider_changed(BTC_slider, null);
        }

        public void ETH_Update_click(object sender, RoutedEventArgs e) {
            UpdateETH();
            ETH_slider_changed(ETH_slider, null);
        }

        public void LTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateLTC();
            LTC_slider_changed(LTC_slider, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetData("BTC-EUR");
            BTC_curr.Text = "Current price: " + App.currency_BTC.ToString() + "€";

            await App.GetHistoricValues(granularityBTC, "BTC-EUR");

            List<ChartDataObject> data = UpdateChartContent(numBTC);
            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        async public Task UpdateETH() {
            await App.GetData("ETH-EUR");
            ETH_curr.Text = "Current price: " + App.currency_ETH.ToString() + "€";

            await App.GetHistoricValues(granularityETH, "ETH-USD");

            List<ChartDataObject> data = UpdateChartContent(numETH);

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        async public Task UpdateLTC() {
            await App.GetData("LTC-EUR");
            LTC_curr.Text = "Current price: " + App.currency_LTC.ToString() + "€";

            await App.GetHistoricValues(granularityLTC, "LTC-USD");

            List<ChartDataObject> data = UpdateChartContent(numLTC);

            AreaSeries series = (AreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private List<ChartDataObject> UpdateChartContent(int num) {

            List<ChartDataObject> data = new List<ChartDataObject>();

            for (int i = 0; i < num; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.pp[i].DateTime, Value = App.pp[i].Low };
                data.Add(obj);
            }

            return data;
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
        private void ETH_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    ETH_diff.Text = "Last hour: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    ETH_DateTimeAxis.MajorStep = 10;
                    granularityETH = 60;
                    numETH = 100;
                    break;
                case 2:
                    ETH_diff.Text = "Last day: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    ETH_DateTimeAxis.MajorStep = 6; ;
                    granularityETH = 900;
                    numETH = 100;
                    break;
                case 3:
                    ETH_diff.Text = "Last week: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    ETH_DateTimeAxis.MajorStep = 1;
                    granularityETH = 3600;
                    numETH = 200;
                    break;
                case 4:
                    ETH_diff.Text = "Last month: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:d/M}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    granularityETH = 14400;
                    numETH = 250;
                    break;
                case 5:
                    ETH_diff.Text = "Last year: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:MMM}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.MinValue;
                    granularityETH = 14400;
                    numETH = 401;
                    break;
                case 6:
                    ETH_diff.Text = "Can't go back in time so far ";
                    ETH_DateTimeAxis.LabelFormat = "{0:MMM}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    granularityETH = 14400;
                    numETH = 401;
                    break;
            }

            UpdateETH();
        }
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
                    numETH = 401;
                    break;
            }

            UpdateLTC();
        }



    }
}
