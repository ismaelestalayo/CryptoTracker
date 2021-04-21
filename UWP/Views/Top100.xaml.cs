using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Controls;
using UWP.APIs;
using UWP.Models;
using UWP.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {

    public sealed partial class Top100 : Page {

        public Top100() {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            vm.GlobalStats = await CoinGecko.GetGlobalStats();
            vm.Top100cards = await CryptoCompare.GetTop100();

            var coinMarket = await Ioc.Default.GetService<ICoinGecko>().GetCoinsMarkets_("EUR");
            //tickers = tickers.OrderBy(x => x.rank).ToList();
            vm.CoinMarket = coinMarket;
        }

        // #########################################################################################
        private void ListView_Click(object sender, ItemClickEventArgs e) {
            top100ListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "listView_Element");

            var crypto = ((Top100card)e.ClickedItem).CoinInfo.Name;
            this.Frame.Navigate(typeof(CoinDetails), crypto);
        }

        private void Pin_click(object sender, RoutedEventArgs e) {
            var card = (Top100card)((FrameworkElement)sender).DataContext;
            var crypto = card.CoinInfo.Name;
            if (!App.pinnedCoins.Contains(crypto)) {
                App.pinnedCoins.Add(crypto);
                vm.InAppNotification($"{crypto} pinned to Home.");
            } else {
                App.pinnedCoins.Remove(crypto);
                vm.InAppNotification($"{crypto} unpinned from Home.");
            }
            card.CoinInfo.FavIcon = card.CoinInfo.FavIcon.Equals("\uEB51") ? "\uEB52" : "\uEB51";

            // Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        private void Fav_click(object sender, RoutedEventArgs e) {
            var card = ((CoinMarket)((FrameworkElement)sender).DataContext);
            var crypto = card.symbol.ToUpperInvariant();
            vm.Vis = Visibility.Collapsed;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var card = ((CoinMarket)((DataGrid)sender).SelectedItem);
            var crypto = card.symbol.ToUpperInvariant();

            top100ListView.PrepareConnectedAnimation("toCoinDetails", null, "listView_Element");
            this.Frame.Navigate(typeof(CoinDetails), crypto);
        }
    }
}
