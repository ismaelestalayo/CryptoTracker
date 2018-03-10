using System;
using System.Collections.Generic;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {
    public sealed partial class ChartUserControl : UserControl {

        //public static readonly DependencyProperty MainContentProperty = 
        //    DependencyProperty.Register("MainContent", typeof(object), typeof(ChartUserControl), new PropertyMetadata(default(object)));

        //public object MainContent {
        //    get { return (object)GetValue(MainContentProperty); }
        //    set { SetValue(MainContentProperty, value); }
        //}

        private int i = 0;

        public ChartUserControl() {
            this.InitializeComponent();
            Update();
            var z = this;
        }

        private async void Update() {
            await App.GetHisto("BTC", "minute", 60);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < App.historic.Count; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject {
                    Date = App.historic[i].DateTime,
                    Value = (App.historic[i].Low + App.historic[i].High) / 2,
                    Low = App.historic[i].Low,
                    High = App.historic[i].High,
                    Open = App.historic[i].Open,
                    Close = App.historic[i].Close,
                    Volume = App.historic[i].Volumefrom
                };
                data.Add(obj);
            }

            SplineAreaSeries series = (SplineAreaSeries)this.radChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Category" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;
        }
    }
}
