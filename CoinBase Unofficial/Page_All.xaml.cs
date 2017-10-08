using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Chart;
using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace CoinBase {
    public sealed partial class Page_All : Page {

        private int limit = 1500;
        private string timeSpan = "hour";

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
            BTC_verticalAxis.Minimum = getMinimum(App.ppBTC);
            BTC_verticalAxis.Maximum = getMaximum(App.ppBTC);
            LoadingControl_BTC.IsLoading = false;
            await GetStats("BTC");
            await Get24hVolume("BTC");

            await UpdateETH();
            ETH_verticalAxis.Minimum = getMinimum(App.ppETH);
            ETH_verticalAxis.Maximum = getMaximum(App.ppETH);
            LoadingControl_ETH.IsLoading = false;
            await GetStats("ETH");
            await Get24hVolume("ETH");

            await UpdateLTC();
            LTC_verticalAxis.Minimum = getMinimum(App.ppLTC);
            LTC_verticalAxis.Maximum = getMaximum(App.ppLTC);
            LoadingControl_LTC.IsLoading = false;
            await GetStats("LTC");
            await Get24hVolume("LTC");
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

            return min * (float)0.993;
        }

        async public Task GetStats(String crypto) {

            await App.GetStats(crypto);

            switch (crypto) {
                case "BTC":
                    BTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    BTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    BTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    BTC_Vol24.Text = App.stats.Volume24 + "BTC";
                    break;

                case "ETH":
                    ETH_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    ETH_High.Text  = App.stats.High24   + App.coinSymbol;
                    ETH_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    ETH_Vol24.Text = App.stats.Volume24 + "ETH";
                    break;

                case "LTC":
                    LTC_Open.Text  = App.stats.Open24   + App.coinSymbol;
                    LTC_High.Text  = App.stats.High24   + App.coinSymbol;
                    LTC_Low.Text   = App.stats.Low24    + App.coinSymbol;
                    LTC_Vol24.Text = App.stats.Volume24 + "LTC";
                    break;

            }
        }
        async private Task Get24hVolume(String crypto) {
            await App.GetHisto(crypto, "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();

            switch (crypto) {
                case "BTC":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.ppBTC[i].DateTime,
                            Volume = App.ppBTC[i].Volumefrom
                        });
                    }
                    this.BTC_VolumeChart.DataContext = data;
                    break;

                case "ETH":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.ppETH[i].DateTime,
                            Volume = App.ppETH[i].Volumefrom
                        });
                    }
                    this.ETH_VolumeChart.DataContext = data;
                    break;

                case "LTC":
                    for (int i = 0; i < 24; i++) {
                        data.Add(new App.ChartDataObject() {
                            Date   = App.ppLTC[i].DateTime,
                            Volume = App.ppLTC[i].Volumefrom
                        });
                    }
                    this.LTC_VolumeChart.DataContext = data;
                    break;
            }
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

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            if (timeSpan.Equals("hour"))
                App.BTC_change1h = dBTC;

            if (dBTC < 0) {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dBTC = Math.Abs(dBTC);
                BTC_diff.Text = "▼" + dBTC.ToString() + "% ";
            } else {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                BTC_diff.Text = "▲" + dBTC.ToString() + "% ";
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

            float dETH = ((App.ETH_now / App.ETH_old) - 1) * 100;
            dETH = (float)Math.Round(dETH, 2);
            if (timeSpan.Equals("hour"))
                App.ETH_change1h = dETH;

            if (dETH < 0) {
                ETH_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dETH = Math.Abs(dETH);
                ETH_diff.Text = "▼" + dETH.ToString() + "% ";
            } else {
                ETH_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                ETH_diff.Text = "▲" + dETH.ToString() + "% ";
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

            float dLTC = ((App.LTC_now / App.LTC_old) - 1) * 100;
            dLTC = (float)Math.Round(dLTC, 2);
            if (timeSpan.Equals("hour"))
                App.LTC_change1h = dLTC;

            if (dLTC < 0) {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dLTC = Math.Abs(dLTC);
                LTC_diff.Text = "▼" + dLTC.ToString() + "% ";
            } else {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                LTC_diff.Text = "▲" + dLTC.ToString() + "% ";
            }

            SplineAreaSeries series = (SplineAreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
    }
}
