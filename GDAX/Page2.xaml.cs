using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace GDAX {
    public sealed partial class Page2 : Page {

        public class PricePoint {
            public string Time { get; set; }
            public float Value { get; set; }
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
            BTC_curr.Text = "Current price: " + App.currency_BTC.ToString();

            await App.GetHistoricValues(3600, "BTC-EUR");

            List<ChartDataObject> data = UpdateChartContent();

            Telerik.UI.Xaml.Controls.Chart.LineSeries series = (Telerik.UI.Xaml.Controls.Chart.LineSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }
        async private Task UpdateETH() {
            await App.GetData("ETH-EUR");
            ETH_curr.Text = "Current price: " + App.currency_ETH.ToString();

            await App.GetHistoricValues(3600, "ETH-EUR");

            //(ETH_Chart.Series[0] as WinRTXamlToolkit.Controls.DataVisualization.Charting.LineSeries).ItemsSource = LoadChartContents();
            List<ChartDataObject> data = UpdateChartContent();

            Telerik.UI.Xaml.Controls.Chart.LineSeries series = (Telerik.UI.Xaml.Controls.Chart.LineSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }

        async private Task UpdateLTC() {
            await App.GetData("LTC-EUR");
            LTC_curr.Text = "Current price: " + App.currency_LTC.ToString();

            await App.GetHistoricValues(3600, "LTC-EUR");

            List<ChartDataObject> data = UpdateChartContent();

            Telerik.UI.Xaml.Controls.Chart.LineSeries series = (Telerik.UI.Xaml.Controls.Chart.LineSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private List<ChartDataObject> UpdateChartContent() {
            List<ChartDataObject> data = new List<ChartDataObject>();

            for (int i = 0; i < 100; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.pp[i].DateTime, Value = App.pp[i].High };
                data.Add(obj);
            }

            return data;
        }

        private List<PricePoint> LoadChartContents() {
            List<PricePoint> lstSource = new List<PricePoint>();
            lstSource.Add(new PricePoint() { Time = App.pp[0].Date.Substring(8, 5), Value = App.pp[0].High });
            lstSource.Add(new PricePoint() { Time = App.pp[1].Date.Substring(8, 5), Value = App.pp[1].High });
            lstSource.Add(new PricePoint() { Time = App.pp[2].Date.Substring(8, 5), Value = App.pp[2].High });
            lstSource.Add(new PricePoint() { Time = App.pp[3].Date.Substring(8, 5), Value = App.pp[3].High });
            lstSource.Add(new PricePoint() { Time = App.pp[4].Date.Substring(8, 5), Value = App.pp[4].High });
            lstSource.Add(new PricePoint() { Time = App.pp[5].Date.Substring(8, 5), Value = App.pp[5].High });

            return lstSource;

        }

    }
}
/*
 *          List<ChartDataObject> dataSouce = new List<ChartDataObject>();

            for (int i = 0; i < 100; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.pp[i].DateTime, Value = App.pp[i].High };
                dataSouce.Add(obj);
            }

            Telerik.UI.Xaml.Controls.Chart.LineSeries series = (Telerik.UI.Xaml.Controls.Chart.LineSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };

            series.ItemsSource = dataSouce;
*/
