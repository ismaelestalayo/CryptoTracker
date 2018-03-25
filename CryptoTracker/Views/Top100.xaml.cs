using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {

    public class GlobalStats {
        public string currency { get; set; }
        public string totalMarketCap { get; set; }
        public string total24Vol { get; set; }
        public string btcDominance { get; set; }
        public string activeCurrencies { get; set; }
    }

    public class Top100coin {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Rank { get; set; }
        public string Price { get; set; }
        public string Vol24 { get; set; }
        public string MarketCap { get; set; }
        public string AvailableSupply { get; set; }
        public string TotalSupply { get; set; }
        public string MaxSupply { get; set; }
        public string Change1h { get; set; }
        public string Change1d { get; set; }
        public string Change7d { get; set; }
        public SolidColorBrush ChangeFG { get; set; }
        public string Src { get; set; }
    }

    public sealed partial class Top100 : Page {

        private GlobalStats g;
        private List<Top100coin> topCoins;

        public Top100() {
            this.InitializeComponent();
            InitPage();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private async void InitPage() {
            g = await App.GetGlobalStats();
            DataContext = g;

            topCoins = await App.GetTop100();
            top100ListView.ItemsSource = topCoins;
        }

        private void ListView_Click(object sender, ItemClickEventArgs e) {
            var clickedItem = (Top100coin)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem.Symbol);
        }

        private void Pin_click(object sender, RoutedEventArgs e) {
            string c = ((Top100coin)((FrameworkElement)((FrameworkElement)sender).Parent).DataContext).Symbol;
            if (!App.pinnedCoins.Contains(c))
                App.pinnedCoins.Add(c);
        }
    }
}
