using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace GDAX {
    public sealed partial class Page2 : Page {

        internal static int granularity = 3600;
        internal static int num;

        public class PricePoint {
            public string Time { get; set; }
            public float Value { get; set; }
        }

        private int _BaudRate = 250;
        public int BaudRate {
            get { return _BaudRate; }
            set { _BaudRate = 250; }
        }

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page2() {
            this.InitializeComponent();

            if (App.firstTime) {
                InitValues();
                App.firstTime = false;
            }


        }

        async private void InitValues() {
            try {
                await UpdateBTC();
                await UpdateETH();
                await UpdateLTC();
            }
            catch {
                ETH_curr.Text = "Maybe you have no internet?";
                BTC_curr.Text = "Maybe you have no internet?";
                LTC_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateBTC();
        }

        private void ETH_Update_click(object sender, RoutedEventArgs e) {
            UpdateETH();
        }

        private void LTC_Update_click(object sender, RoutedEventArgs e) {
            UpdateLTC();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async private Task UpdateBTC() {
            await App.GetData("BTC-EUR");
            BTC_curr.Text = "Current price: " + App.currency_BTC.ToString() + "€";

            await App.GetHistoricValues(granularity, "BTC-EUR");

            List<ChartDataObject> data = UpdateChartContent();

            LineSeries series = (LineSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding    = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }
        async private Task UpdateETH() {
            await App.GetData("ETH-EUR");
            ETH_curr.Text = "Current price: " + App.currency_ETH.ToString() + "€";

            await App.GetHistoricValues(granularity, "ETH-EUR");
            
            List<ChartDataObject> data = UpdateChartContent();

            LineSeries series = (LineSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding    = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }

        async private Task UpdateLTC() {
            await App.GetData("LTC-EUR");
            LTC_curr.Text = "Current price: " + App.currency_LTC.ToString() + "€";

            await App.GetHistoricValues(granularity, "LTC-EUR");

            List<ChartDataObject> data = UpdateChartContent();

            LineSeries series = (LineSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding    = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private List<ChartDataObject> UpdateChartContent() {

            num = 200;

            List<ChartDataObject> data = new List<ChartDataObject>();

            for (int i = 0; i < num; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.pp[i].DateTime, Value = App.pp[i].High };
                data.Add(obj);
            }

            return data;
        }

    }
}
