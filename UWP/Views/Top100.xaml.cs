using Microsoft.Toolkit.Mvvm.DependencyInjection;
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

            vm.CoinMarket = await Ioc.Default.GetService<ICoinGecko>().GetCoinsMarkets_("EUR");
            //tickers = tickers.OrderBy(x => x.rank).ToList();
        }

        // #########################################################################################
        private void ListView_Click(object sender, ItemClickEventArgs e) {
            CurrenciesListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "listView_Element2");

            var crypto = ((CoinMarket)e.ClickedItem).symbol.ToUpperInvariant();
            // ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("toCoinDetails", sender);
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
        }

    }
}
