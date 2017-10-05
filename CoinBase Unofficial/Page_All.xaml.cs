using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Chart;
using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace CoinBase {
    public sealed partial class Page_All : Page {

        private int limit = 1500;

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_All() {
            this.InitializeComponent();

            UpdateHome();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task UpdateHome() {

            if (LoadingControl_BTC == null)
                LoadingControl_BTC = new Loading();
            if (LoadingControl_ETH == null)
                LoadingControl_ETH = new Loading();
            if (LoadingControl_LTC == null)
                LoadingControl_LTC = new Loading();

            LoadingControl_BTC.IsLoading = true;
            LoadingControl_ETH.IsLoading = true;
            LoadingControl_LTC.IsLoading = true;

            await UpdateBTC();
            BTC_verticalAxis.Maximum = getMaximum(App.ppBTC);
            BTC_verticalAxis.Minimum = getMinimum(App.ppBTC);
            LoadingControl_BTC.IsLoading = false;

            await UpdateETH();
            ETH_verticalAxis.Maximum = getMaximum(App.ppETH);
            ETH_verticalAxis.Minimum = getMinimum(App.ppETH);
            LoadingControl_ETH.IsLoading = false;

            await UpdateLTC();
            LTC_verticalAxis.Maximum = getMaximum(App.ppLTC);
            LTC_verticalAxis.Minimum = getMinimum(App.ppLTC);
            LoadingControl_LTC.IsLoading = false;

        }

        private float getMaximum(List<App.PricePoint> a) {
            int i = 0;
            float max = 0;

            foreach (App.PricePoint type in a) {
                if (a[i].High > max)
                    max = a[i].High;

                i++;
            }

            return max;
        }
        private float getMinimum(List<App.PricePoint> a) {
            int i = 0;
            float min = 9000;

            foreach (App.PricePoint type in a) {
                if (a[i].High < min)
                    min = a[i].High;

                i++;
            }
            
            return min * (float)0.999;
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = App.BTC_now.ToString() + App.coinSymbol;

            await App.GetHisto("BTC", "minute", limit);                    

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject {
                    Date = App.ppBTC[i].DateTime,
                    Value = App.ppBTC[i].Low
                };
                data.Add(obj);
            }

            SplineAreaSeries series = (SplineAreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = App.ETH_now.ToString() + App.coinSymbol;

            await App.GetHisto("ETH", "minute", limit);

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppETH[i].DateTime, Value = App.ppETH[i].Low };
                data.Add(obj);
            }

            SplineAreaSeries series = (SplineAreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
        async public Task UpdateLTC() {
            await App.GetCurrentPrice("LTC");
            LTC_curr.Text = App.LTC_now.ToString() + App.coinSymbol;

            await App.GetHisto("LTC", "minute", limit);

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject {
                    Date = App.ppLTC[i].DateTime,
                    Value = App.ppLTC[i].Low
                };
                data.Add(obj);
            }

            SplineAreaSeries series = (SplineAreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
    }
}
