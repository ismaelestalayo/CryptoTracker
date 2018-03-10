using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// Without this link I wouldn't have made it: http://blog.jerrynixon.com/2013/07/solved-two-way-binding-inside-user.html

namespace CryptoTracker.Views {
    public sealed partial class ChartUserControl : UserControl {

        public static readonly DependencyProperty CryptoProperty = 
            DependencyProperty.Register(
                  "Crypto", typeof(string),
                  typeof(ChartUserControl),
                  new PropertyMetadata("null") );

        public string Crypto {
            get { return (string)GetValue(CryptoProperty); }
            set { SetValueDp(CryptoProperty, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDp(DependencyProperty property, object value,
            [System.Runtime.CompilerServices.CallerMemberName] String p = null) {
            SetValue(property, value);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        public ChartUserControl() {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
            Update();
        }

        private async void Update() {
            string c = Crypto;
            int ii = 0;
            while (c.Equals("null")) {
                await Task.Delay(TimeSpan.FromMilliseconds(200) );
                c = Crypto;
                ii++;
            }
            await App.GetHisto(c, "minute", 60);

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

            try {
                splineAreaSerie.Fill   = (SolidColorBrush)Application.Current.Resources[c + "_colorT"];
                splineAreaSerie.Stroke = (SolidColorBrush)Application.Current.Resources[c + "_color"];
            } catch (Exception) {
                splineAreaSerie.Fill   = (SolidColorBrush)Application.Current.Resources["null_colorT"];
                splineAreaSerie.Stroke = (SolidColorBrush)Application.Current.Resources["null_color"];
            }
        }
    }
}
