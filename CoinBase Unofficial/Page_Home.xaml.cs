using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
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

        async private void InitValues() {
            try {
                await UpdateBTC();
                await UpdateETH();
                await UpdateLTC();

            } catch (Exception ex) {
                ETH_curr.Text = "Error! ";
                BTC_curr.Text = ex.StackTrace;
                LTC_curr.Text = ex.Message;
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
            await App.GetCurrentPrice("BTC");
            BTC_curr.Text = App.BTC_now.ToString();
            if (App.coin.Equals("EUR"))
                BTC_curr.Text += "€";
            else {
                BTC_curr.Text += "$";
            }

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
            if (dBTC < 0) {
                BTC_diff.Foreground = BTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
                BTC_difff.Text = "\xEB0F"; //C# parser works different from XAML parser
                dBTC = Math.Abs(dBTC);
            } else {
                BTC_diff.Foreground = BTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
                BTC_difff.Text = "\xEB11";
            }
            BTC_diff.Text = dBTC.ToString() + "%";

            AreaSeries series = (AreaSeries)BTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = App.ETH_now.ToString();
            if (App.coin.Equals("EUR"))
                ETH_curr.Text += "€";
            else {
                ETH_curr.Text += "$";
            }

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
            if (dETH < 0) {
                ETH_diff.Foreground = ETH_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
                ETH_difff.Text = "\xEB0F"; //C# parser works different from XAML parser
                dETH = Math.Abs(dETH);
            } else {
                ETH_diff.Foreground = ETH_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
                ETH_difff.Text = "\xEB11";
            }
            ETH_diff.Text = dETH.ToString() + "%";

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        async public Task UpdateLTC() {
            await App.GetCurrentPrice("LTC");
            LTC_curr.Text = App.LTC_now.ToString();

            if (App.coin.Equals("EUR"))
                LTC_curr.Text += "€";
            else {
                LTC_curr.Text += "$";
            }

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
            if (dLTC < 0) {
                LTC_diff.Foreground = LTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
                LTC_difff.Text = "\xEB0F"; //C# parser works different from XAML parser
                dLTC = Math.Abs(dLTC);
            } else {
                LTC_diff.Foreground = LTC_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
                LTC_difff.Text = "\xEB11";
            }
            LTC_diff.Text = dLTC.ToString() + "%";

            AreaSeries series = (AreaSeries)LTC_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void BTC_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    BTC_from.Text = "Last hour: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    BTC_DateTimeAxis.MajorStep = 10;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case 2:
                    BTC_from.Text = "Last day: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    BTC_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case 3:
                    BTC_from.Text = "Last week: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case 4:
                    BTC_from.Text = "Last month: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;

                case 5:
                    BTC_from.Text = "Last year: ";
                    BTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    BTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    BTC_DateTimeAxis.MajorStep = 1;
                    BTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;

                case 6:
                    BTC_from.Text = "Sorry, can't go back in time so far ";
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
        private void ETH_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    ETH_from.Text = "Last hour: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    ETH_DateTimeAxis.MajorStep = 10;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case 2:
                    ETH_from.Text = "Last day: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    ETH_DateTimeAxis.MajorStep = 6;
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case 3:
                    ETH_from.Text = "Last week: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case 4:
                    ETH_from.Text = "Last month: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:d/M}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;

                case 5:
                    ETH_from.Text = "Last year: ";
                    ETH_DateTimeAxis.LabelFormat = "{0:MMM}";
                    ETH_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    ETH_DateTimeAxis.MajorStep = 1;
                    ETH_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "year";
                    limit = 365;
                    break;
                case 6:
                    ETH_from.Text = "Sorry, can't go back in time so far ";
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
        private void LTC_slider_changed(object sender, RangeBaseValueChangedEventArgs e) {
            Slider s = (Slider)sender;
            switch (s.Value) {
                case 1:
                    LTC_from.Text = "Last hour: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    LTC_DateTimeAxis.MajorStep = 10;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    timeSpan = "hour";
                    limit = 60;
                    break;
                case 2:
                    LTC_from.Text = "Last day: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    LTC_DateTimeAxis.MajorStep = 6;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case 3:
                    LTC_from.Text = "Last week: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:ddd d}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    timeSpan = "week";
                    limit = 168;
                    break;

                case 4:
                    LTC_from.Text = "Last month: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:d/M}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    timeSpan = "month";
                    limit = 744;
                    break;

                case 5:
                    LTC_from.Text = "Last year: ";
                    LTC_DateTimeAxis.LabelFormat = "{0:MMM}";
                    LTC_DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    LTC_DateTimeAxis.MajorStep = 1;
                    LTC_DateTimeAxis.Minimum = DateTime.MinValue;
                    timeSpan = "all";
                    limit = 0;
                    break;

                case 6:
                    LTC_from.Text = "Can't go back in time so far ";
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
