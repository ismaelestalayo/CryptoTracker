using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_Home : Page {

        internal int limit = 60;
        private  string timeSpan = "hour";

        public class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
        }

        public Page_Home() {
            this.InitializeComponent();
            InitValues();

        }

        private void InitValues() {
            try {
                BTC_Update_click(null, null);
                ETH_Update_click(null, null);
                LTC_Update_click(null, null);

            } catch (Exception ex) {
                LoadingControl_BTC.IsLoading = false;
                LoadingControl_ETH.IsLoading = false;
                LoadingControl_LTC.IsLoading = false;
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
                ETH_curr.Text = "Error!";
                BTC_curr.Text = "Error!";
                LTC_curr.Text = "Error!";
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void BTC_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl_BTC == null)
                LoadingControl_BTC = new Loading();
            
            LoadingControl_BTC.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = "hour" };
            BTC_TimerangeButton_Click(r, null);
        }

        public void ETH_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl_ETH == null)
                LoadingControl_ETH = new Loading();
            
            LoadingControl_ETH.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = "hour" };
            ETH_TimerangeButton_Click(r, null);
        }

        public void LTC_Update_click(object sender, RoutedEventArgs e) {
            if (LoadingControl_LTC == null)
                LoadingControl_LTC = new Loading();
            
            LoadingControl_LTC.IsLoading = true;
            
            RadioButton r = new RadioButton { Content = "hour" };
            LTC_TimerangeButton_Click(r, null);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateBTC() {
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = (App.coin.Equals("EUR")) ? App.BTC_now.ToString() + "€" : App.BTC_now.ToString() + "$";

            switch (timeSpan) {
                case "hour":
                    await App.GetHisto("BTC", "minute", limit);
                    break;
                case "day":
                    await App.GetHisto("BTC", "minute", limit);
                    break;
                case "week":
                    await App.GetHisto("BTC", "hour", limit);
                    break;
                case "month":
                    await App.GetHisto("BTC", "hour", limit);
                    break;
                case "year":
                    await App.GetHisto("BTC", "day", limit);
                    break;
                case "all":
                    await App.GetHisto("BTC", "day", 0);
                    break;
            }

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppBTC[i].DateTime, Value = App.ppBTC[i].Low };
                data.Add(obj);
            }

            float dBTC = ((App.BTC_now / App.BTC_old) - 1) * 100;
            dBTC = (float)Math.Round(dBTC, 2);
            if (timeSpan.Equals("hour")) 
                App.BTC_change1h = dBTC;

            if (dBTC < 0) {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dBTC = Math.Abs(dBTC);
                BTC_diff.Text = "▼" + dBTC.ToString() + " % ";
            } else {
                BTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                BTC_diff.Text = "▲" + dBTC.ToString() + " % ";
            }

            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_BTC.IsLoading = false;
        }
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = (App.coin.Equals("EUR")) ? App.ETH_now.ToString() + "€" : App.ETH_now.ToString() + "$";

            switch (timeSpan) {
                case "hour":
                    await App.GetHisto("ETH",  "minute", limit);
                    break;
                case "day":
                    await App.GetHisto("ETH", "minute", limit);
                    break;
                case "week":
                    await App.GetHisto("ETH", "hour", limit);
                    break;
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
                ETH_diff.Foreground  = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dETH = Math.Abs(dETH);
                ETH_diff.Text = "▼" + dETH.ToString() + " % ";
            } else {
                ETH_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                ETH_diff.Text = "▲" + dETH.ToString() + " % ";
            }            

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_ETH.IsLoading = false;
        }
        async public Task UpdateLTC() {
            await App.GetCurrentPrice("LTC");
            LTC_curr.Text = (App.coin.Equals("EUR")) ? App.LTC_now.ToString() + "€" : App.LTC_now.ToString() + "$";

            switch (timeSpan) {
                case "hour":
                    await App.GetHisto("LTC", "minute", limit);
                    break;
                case "day":
                    await App.GetHisto("LTC", "minute", limit);
                    break;
                case "week":
                    await App.GetHisto("LTC", "hour", limit);
                    break;
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

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppLTC[i].DateTime, Value = App.ppLTC[i].Low };
                data.Add(obj);
            }

            float dLTC = ((App.LTC_now / App.LTC_old) - 1) * 100;
            dLTC = (float)Math.Round(dLTC, 2);
            if (timeSpan.Equals("hour")) 
                App.LTC_change1h = dLTC;

            if (dLTC < 0) {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 180, 0, 0));
                dLTC = Math.Abs(dLTC);
                LTC_diff.Text = "▼" + dLTC.ToString() + " % ";
            } else {
                LTC_diff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 120, 0));
                LTC_diff.Text = "▲" + dLTC.ToString() + " % ";
            }

            AreaSeries series = (AreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
            LoadingControl_LTC.IsLoading = false;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl_BTC.IsLoading = true;
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    BTC_DateTimeAxis.MajorStep = 10;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    BTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    BTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    BTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Today.AddMonths(-4);
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdateBTC();
        }
        private void ETH_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl_ETH.IsLoading = true;
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
        private void LTC_TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl_LTC.IsLoading = true;
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
