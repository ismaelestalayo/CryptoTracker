using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_ETH : Page {
        
        internal static int limit = 60;
        internal static string timeSpan = "day";

        public Page_ETH() {
            this.InitializeComponent();
            InitValues();
        }

        async private void InitValues() {
            try {
                ETH_Update_click(null, null);
                //await GetStats();
                //await Get24Volume();

            } catch (Exception ex) {
                LoadingControl.IsLoading = false;
                ETH_curr.Text = "Error!";
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For SyncAll button
        public void ETH_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = timeSpan };
            ETH_TimerangeButton_Click(r, null);
            GetStats();
            Get24Volume();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = (App.coin.Equals("EUR")) ? App.ETH_now.ToString() + "€" : App.ETH_now.ToString() + "$";

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("ETH", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("ETH", "hour", limit);
                    break;

                case "year":
                    await App.GetHisto("ETH", "day", limit);
                    break;

                case "all":
                    await App.GetHisto("ETH", "day", 0);
                    break;
            }

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject { Date   = App.ppETH[i].DateTime,
                                                            Value  =(App.ppETH[i].Low + App.ppETH[i].High) / 2,
                                                            Low    = App.ppETH[i].Low,
                                                            High   = App.ppETH[i].High,
                                                            Open   = App.ppETH[i].Open,
                                                            Close  = App.ppETH[i].Close,
                                                            Volume = App.ppETH[i].Volumefrom
                };
                data.Add(obj);

            }

            float dETH = ((App.ETH_now / App.ETH_old) - 1) * 100;
            dETH = (float)Math.Round(dETH, 2);
            if (timeSpan.Equals("hour"))
                App.ETH_change1h = dETH;

            if (dETH < 0) {
                ETH_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dETH = Math.Abs(dETH);
                ETH_diff.Text = "▼" + dETH.ToString() + "%";
            } else {
                ETH_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                ETH_diff.Text = "▼" + dETH.ToString() + "%";
            }

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl.IsLoading = false;
        }

        async public Task GetStats() {

            await App.GetStats("ETH");

            string sym = (App.coin.Equals("EUR")) ? "€" : "$";

            ETH_Open.Text  = App.stats.Open24 + sym;
            ETH_High.Text  = App.stats.High24 + sym;
            ETH_Low.Text   = App.stats.Low24  + sym;
            ETH_Vol24.Text = App.stats.Volume24 + "ETH";
            
        }
        async private Task Get24Volume() {
            await App.GetHisto("ETH", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add(new App.ChartDataObject() {
                    Date = App.ppETH[i].DateTime,
                    Volume = App.ppETH[i].Volumefrom
                });
            }
            this.VolumeChart.DataContext = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ETH_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl.IsLoading = true;
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    ETH_DateTimeAxis.MajorStep = 10;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    ETH_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    ETH_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    ETH_DateTimeAxis.LabelFormat = "{0:d/M}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    ETH_DateTimeAxis.LabelFormat = "{0:MMM}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    ETH_DateTimeAxis.LabelFormat = "{0:MMM}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateETH();
        }
    }
}
