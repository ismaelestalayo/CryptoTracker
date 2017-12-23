using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {
    public sealed partial class Page_ETH : Page {
        
        private static int limit = 60;
        private static string timeSpan = "day";

        public Page_ETH() {
            this.InitializeComponent();
            InitValues();

            //TimeSpan period = TimeSpan.FromSeconds(30);
            //ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => {
            //    Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
            //        RadioButton r = new RadioButton { Content = timeSpan };
            //        if (timeSpan == "hour")
            //            ETH_TimerangeButton_Click(r, null);
            //    });
            //}, period);

        }

        async private void InitValues() {
            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                ETH_TimerangeButton_Click(r, null);

            } catch (Exception) {
                LoadingControl.IsLoading = false;
                ETH_curr.Text = "Error!";
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For Sync button //////////////////////////////////////////////////////////////////////////////////
        internal async void UpdatePage() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;

            await UpdateETH();
            ETH_verticalAxis.Minimum = getMinimum(App.ppETH);
            ETH_verticalAxis.Maximum = getMaximum(App.ppETH);
            ETH_DateTimeAxis = App.AdjustAxis(ETH_DateTimeAxis, timeSpan);
            await GetStats();
            await Get24Volume();
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
            float min = 15000;

            foreach (App.PricePoint type in a) {
                if (a[i].High < min)
                    min = a[i].High;
                i++;
            }
            return min * (float)0.99;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = App.ETH_now.ToString() + App.coinSymbol;

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
                ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                dETH = Math.Abs(dETH);
                ETH_diff.Text = "▼" + dETH.ToString() + "%";
            } else {
                ETH_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                ETH_diff.Text = "▼" + dETH.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        async public Task GetStats() {

            await App.GetStats("ETH");

            ETH_Open.Text  = App.stats.Open24 + App.coinSymbol;
            ETH_High.Text  = App.stats.High24 + App.coinSymbol;
            ETH_Low.Text   = App.stats.Low24  + App.coinSymbol;
            ETH_Vol24.Text = App.stats.Volume24 + "ETH";
            
        }
        async private Task Get24Volume() {
            await App.GetHisto("ETH", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add( new App.ChartDataObject() {
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
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "month";
                    limit = 744;
                    break;

                case "year":
                    timeSpan = "year"; 
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdatePage();
        }
    }
}
