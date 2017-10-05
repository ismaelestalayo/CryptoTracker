using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Telerik.UI.Xaml.Controls.Chart;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace CoinBase {
    public sealed partial class Page_All : Page {

        public Page_All() {
            this.InitializeComponent();

            List<App.ChartDataObject> dataBTC = new List<App.ChartDataObject>();
            List<App.ChartDataObject> dataETH = new List<App.ChartDataObject>();
            List<App.ChartDataObject> dataLTC = new List<App.ChartDataObject>();

            for (int i = 0; i < App.ppBTC.Count - 1; i++) {
                dataBTC.Add(new App.ChartDataObject() {
                    Date  = App.ppBTC[i].DateTime,
                    Value = App.ppBTC[i].Low,
                    Low   = App.ppBTC[i].Low,
                    High  = App.ppBTC[i].High
                });
            }

            SplineAreaSeries BTCserie = (SplineAreaSeries)AllChartt.Series[0];
            BTCserie.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            BTCserie.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            BTCserie.ItemsSource = dataBTC;
            //dataBTC.Clear();

            for (int i = 0; i < App.ppBTC.Count - 1; i++) {
                dataETH.Add(new App.ChartDataObject() {
                    Date   = App.ppETH[i].DateTime,
                    Value  = App.ppETH[i].Low,
                    Low    = App.ppETH[i].Low,
                    High   = App.ppETH[i].High
                });
            }

            SplineAreaSeries ETHserie = (SplineAreaSeries)AllChartt.Series[1];
            ETHserie.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            ETHserie.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            ETHserie.ItemsSource = dataETH;
            //dataETH.Clear();

            for (int i = 0; i < App.ppBTC.Count - 1; i++) {
                dataLTC.Add(new App.ChartDataObject() {
                    Date   = App.ppLTC[i].DateTime,
                    Value  = App.ppLTC[i].Low,
                    Low    = App.ppLTC[i].Low,
                    High   = App.ppLTC[i].High
                });
            }

            SplineAreaSeries LTCserie = (SplineAreaSeries)AllChartt.Series[2];
            LTCserie.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            LTCserie.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            LTCserie.ItemsSource = dataLTC;
            //dataLTC.Clear();


        }

        private void Chart_BTC(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {
                AllChartt.Series[0].Visibility = Windows.UI.Xaml.Visibility.Visible;

            } else {
                AllChartt.Series[0].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void Chart_ETH(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {
                AllChartt.Series[1].Visibility = Windows.UI.Xaml.Visibility.Visible;

            } else {
                AllChartt.Series[1].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void Chart_LTC(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {
                AllChartt.Series[2].Visibility = Windows.UI.Xaml.Visibility.Visible;

            } else {
                AllChartt.Series[2].Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
