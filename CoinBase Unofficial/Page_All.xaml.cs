using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace CoinBase {
    public sealed partial class Page_All : Page {

        public Page_All() {
            this.InitializeComponent();
        }

        private void Chart_BTC(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {

                List<App.ChartDataObject> data = new List<App.ChartDataObject>();
                for (int i = 0; i < 24; i++) {
                    data.Add(new App.ChartDataObject() {
                        Date = App.ppBTC[i].DateTime,
                        Low  = App.ppBTC[i].Low,
                        High = App.ppBTC[i].High
                    });
                }

                AllChart.DataContext = data;
            }
        }

        private void Chart_ETH(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {
                

                List<App.ChartDataObject> data = new List<App.ChartDataObject>();
                for (int i = 0; i < 24; i++) {
                    data.Add(new App.ChartDataObject() {
                        Date = App.ppETH[i].DateTime,
                        Low  = App.ppETH[i].Low,
                        High = App.ppETH[i].High
                    });
                }

                AllChart.DataContext = data;
            }
        }

        private void Chart_LTC(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            CheckBox c = sender as CheckBox;
            if (c.IsChecked == true) {

                List<App.ChartDataObject> data = new List<App.ChartDataObject>();
                for (int i = 0; i < 24; i++) {
                    data.Add(new App.ChartDataObject() {
                        Date = App.ppLTC[i].DateTime,
                        Low  = App.ppLTC[i].Low,
                        High = App.ppLTC[i].High
                    });
                }

                AllChart.DataContext = data;
            }
        }
    }
}
