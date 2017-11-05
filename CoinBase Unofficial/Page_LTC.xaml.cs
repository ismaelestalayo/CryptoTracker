using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase
{
    public sealed partial class Page_LTC : Page {

        internal static int limit = 60;
        internal static string timeSpan = "day";

        public Page_LTC() {
            this.InitializeComponent();
            InitValues();

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) => {
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    LTC_TimerangeButton_Click(r, null);
                });
            }, period);
        }

        async private void InitValues() {
            try {
                LTC_Update_click();

            } catch (Exception ex) {
                LoadingControl.IsLoading = false;
                LTC_curr.Text = "Error!";
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For SyncAll button
        public void LTC_Update_click() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = timeSpan };
            LTC_TimerangeButton_Click(r, null);
            GetStats();
            Get24Volume();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateLTC() {
            await App.GetCurrentPrice("LTC");
            LTC_curr.Text = App.LTC_now.ToString() + App.coinSymbol;

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("LTC", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("LTC", "hour", limit);
                    break;

                case "year":
                    await App.GetHisto("LTC", "day", limit);
                    break;

                case "all":
                    await App.GetHisto("LTC", "day", 0);
                    break;
            }

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject { Date   = App.ppLTC[i].DateTime,
                                                            Value  = (App.ppLTC[i].Low + App.ppLTC[i].High) / 2,
                                                            Low    = App.ppLTC[i].Low,
                                                            High   = App.ppLTC[i].High,
                                                            Open   = App.ppLTC[i].Open,
                                                            Close  = App.ppLTC[i].Close,
                                                            Volume = App.ppLTC[i].Volumefrom
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

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        async private Task GetStats() {

            await App.GetStats("LTC");

            LTC_Open.Text  = App.stats.Open24 + App.coinSymbol;
            LTC_High.Text  = App.stats.High24 + App.coinSymbol;
            LTC_Low.Text   = App.stats.Low24  + App.coinSymbol;
            LTC_Vol24.Text = App.stats.Volume24 + "LTC";
        }

        async private Task Get24Volume() {
            await App.GetHisto("LTC", "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add( new App.ChartDataObject() {
                    Date = App.ppLTC[i].DateTime,
                    Volume = App.ppLTC[i].Volumefrom
                });
            }
            this.VolumeChart.DataContext = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void LTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl.IsLoading = true;
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    LTC_DateTimeAxis.MajorStep = 10;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    LTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    LTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    LTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateLTC();
        }
    }
}
