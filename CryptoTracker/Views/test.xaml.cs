using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {
    public sealed partial class test : Page {

        static ObservableCollection<homeCoinsClass> homeCoinList { get; set; }
        public ObservableCollection<ChartPlotInfo> ChartPlotInfos { get; set; }
        private string diff;
        private string crypto = "BTC";

        public test() {
            this.InitializeComponent();

            homeCoinList = new ObservableCollection<homeCoinsClass>();

            updateChartAsync("BTC").ConfigureAwait(true);
            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task updateChartAsync(string crypto) {
            await App.GetHisto(crypto, "minute", 60);

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

            float d = 0;
            float oldestPrice = App.historic[0].Close;
            float newestPrice = App.historic[App.historic.Count - 1].Close;
            d = (float)Math.Round(((newestPrice / oldestPrice) - 1) * 100, 2);

            if (d < 0) {
            //    BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                d = Math.Abs(d);
                diff = "▼" + d.ToString() + "%";
            } else {
            //    BTC_diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                diff = "▲" + d.ToString() + "%";
            }

            SplineAreaSeries spline = new SplineAreaSeries();
            spline.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            spline.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            spline.ItemsSource = data;

            //RadCartesianChart chart = new RadCartesianChart();
            //chart.HorizontalAxis = new CategoricalAxis();
            //chart.VerticalAxis = new LinearAxis();
            //chart.Series.Add(spline);
            

            homeCoinList.Add(new homeCoinsClass {
                _cryptoName = crypto,
                _priceCurr = App.GetCurrentPrice(crypto, "defaultMarket").ToString() + App.coinSymbol,
                _priceDiff = diff,
                _splineAreaSeries = spline
            });

            

            homeListView.ItemsSource = homeCoinList;
        }


    }

    public class ChartPlotInfo {
        public double Value { get; set; }
        public string Category { get; set; }
    }
}
