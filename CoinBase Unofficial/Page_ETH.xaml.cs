using CoinBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace CoinBase {
    public sealed partial class Page_ETH : Page {
        
        internal static int granularityETH = 3600;
        internal static int numETH = 60;


        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_ETH() {
            this.InitializeComponent();
            InitValues();
        }

        async private void InitValues() {
            try {
                await UpdateETH();
                await GetStats("ETH-EUR");

            } catch (Exception ex) {
                ETH_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For SyncAll button
        public void ETH_Update_click(object sender, RoutedEventArgs e) { 
            UpdateETH();
            ETH_slider_changed(ETH_slider, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateETH() {
            await App.GetData("ETH-EUR");
            ETH_curr.Text = "Current price: " + App.currency_ETH.ToString() + "€";

            await App.GetHistoricValues(granularityETH, "ETH-USD");

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < numETH; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppETH[i].DateTime, Value = App.ppETH[i].Low };
                data.Add(obj);
            }

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        async public Task GetStats(string currency_pair) {
            HttpClient client = new HttpClient();

            String URL = "https://api.gdax.com/products/" + currency_pair + "/stats";
            String response;
            try {
                response = await client.GetStringAsync(URL);
            } catch {
                response = " ";
            }
            var data = JObject.Parse(response);

            ETH_Volume30.Text = "Volume last 30 days:" + data["volume_30day"].ToString();
            ETH_High.Text = "High:" + data["high"].ToString();
            ETH_Low.Text = "Low:" + data["low"].ToString();
            ETH_Open.Text = "Open:" + data["open"].ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ETH_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    ETH_diff.Text = "Last hour: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    ETH_DateTimeAxis.MajorStep = 10;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    granularityETH = 60;
                    numETH = 60;
                    break;
                case 2:
                    ETH_diff.Text = "Last day: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    ETH_DateTimeAxis.MajorStep = 6;
                    granularityETH = 900;
                    numETH = 100;
                    break;
                case 3:
                    ETH_diff.Text = "Last week: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
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
                    ETH_diff.Text = "Sorry, can't go back in time so far ";
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
    }
}
