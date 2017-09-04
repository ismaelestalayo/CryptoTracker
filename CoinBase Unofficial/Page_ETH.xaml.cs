﻿using CoinBase;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class Page_ETH : Page {
        
        internal static int limit = 60;
        private static string timeSpan = "day";

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
                await GetStats();

            } catch (Exception ex) {
                ETH_curr.Text = "Maybe you have no internet?";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //For SyncAll button
        public void ETH_Update_click(object sender, RoutedEventArgs e) {
            UpdateETH();
            ETH_slider_changed(ETH_slider, null);
            GetStats();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async public Task UpdateETH() {
            await App.GetCurrentPrice("ETH");
            ETH_curr.Text = "Current price: " + App.ETH_now.ToString();
            if (App.coin.Equals("EUR"))
                ETH_curr.Text += "€";
            else {
                ETH_curr.Text += "$";
            }

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto("ETH", "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto("ETH", "hour",   limit);
                    break;

                case "year":
                    await App.GetHisto("ETH", "day",    limit);
                    break;

                case "all":
                    await App.GetHisto("ETH", "day",    0);
                    break;
            }

            List<ChartDataObject> data = new List<ChartDataObject>();
            for (int i = 0; i < limit; ++i) {
                ChartDataObject obj = new ChartDataObject { Date = App.ppETH[i].DateTime, Value = App.ppETH[i].Low };
                data.Add(obj);
            }

            float dETH = ((App.ETH_now / App.ETH_old) - 1) * 100;
            dETH = (float)Math.Round(dETH, 2);
            ETH_diff.Text = dETH.ToString() + "%";
            if (dETH < 0) {
                ETH_diff.Foreground = ETH_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 127, 0, 0));
                ETH_difff.Text = "\xEB0F"; //C# parser works different from XAML parser
            } else {
                ETH_diff.Foreground = ETH_difff.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 127, 0));
                ETH_difff.Text = "\xEB11";
            }

            AreaSeries series = (AreaSeries)ETH_Chart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }

        async public Task GetStats() {

            await App.GetStats("ETH");

            string sym;
            if (App.coin.Equals("EUR")) { 
                sym = "€";
            } else{ 
                sym = "$";
            }

            ETH_Open.Text  = App.stats.Open24 + sym;
            ETH_High.Text  = App.stats.High24 + sym;
            ETH_Low.Text   = App.stats.Low24 + sym;
            ETH_Vol24.Text = App.stats.Volume24 + "ETH";
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    timeSpan = "all";
                    limit = 0;
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

    }
}