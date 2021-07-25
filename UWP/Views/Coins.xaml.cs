using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Services;
using UWP.Shared.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {

    public sealed partial class Coins : Page, UpdatablePage {

        private string sortedBy = "";

        public Coins() {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            await UpdatePage();
        }

        public async Task UpdatePage() {
            var coinMarket = await LocalStorageHelper.ReadObject<List<CoinMarket>>(UserStorage.CoinsCache);
            vm.CoinMarket = new ObservableCollection<CoinMarket>(coinMarket);

            vm.GlobalStats = await CoinGecko.GetGlobalStats();

            var market = await Ioc.Default.GetService<ICoinGecko>().GetCoinsMarkets_();
            market = market.OrderBy(x => x.market_cap_rank).ToList();
            vm.CoinMarket = new ObservableCollection<CoinMarket>(market);

            await LocalStorageHelper.SaveObject(UserStorage.CoinsCache, vm.CoinMarket);
        }

        // #########################################################################################
        private void ListView_Click(object sender, ItemClickEventArgs e) {
            CurrenciesListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "listView_Element2");
            
            var crypto = ((CoinMarket)e.ClickedItem).symbol.ToUpperInvariant();
            // ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("toCoinDetails", sender);
            this.Frame.Navigate(typeof(CoinDetails), crypto);
        }

        private void Fav_click(object sender, RoutedEventArgs e) {
            var card = (CoinMarket)((FrameworkElement)sender).DataContext;
            var crypto = card.symbol.ToUpperInvariant();
            if (!App.pinnedCoins.Contains(crypto)) {
                App.pinnedCoins.Add(crypto);
                vm.InAppNotification($"{crypto} pinned to Home.");
            }
            else {
                App.pinnedCoins.Remove(crypto);
                vm.InAppNotification($"{crypto} unpinned from Home.");
            }
            card.IsFav = !card.IsFav; // card.FavIcon.Equals("\uEB51") ? "\uEB52" : "\uEB51";

            // Update pinnedCoin list
            App.UpdatePinnedCoins();
        }

        private void Sort_list_click(object sender, RoutedEventArgs e) {
            var btn = ((ContentControl)sender).Content.ToString().ToLowerInvariant();
            var list = new List<CoinMarket>();
            switch (btn) {
                case "rank":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderBy(x => x.market_cap_rank).ToList() :
                        vm.CoinMarket.OrderByDescending(x => x.market_cap_rank).ToList();
                    break;
                case "price":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.current_price).ToList() :
                        vm.CoinMarket.OrderBy(x => x.current_price).ToList();
                    break;
                case "mkt. cap.":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.market_cap).ToList() :
                        vm.CoinMarket.OrderBy(x => x.market_cap).ToList();
                    break;
                case "volume":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.total_volume).ToList() :
                        vm.CoinMarket.OrderBy(x => x.total_volume).ToList();
                    break;
                case "24h":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.price_change_percentage_24h_in_currency).ToList() :
                        vm.CoinMarket.OrderBy(x => x.price_change_percentage_24h_in_currency).ToList();
                    break;
                case "7d":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.price_change_percentage_7d_in_currency).ToList() :
                        vm.CoinMarket.OrderBy(x => x.price_change_percentage_7d_in_currency).ToList();
                    break;
                case "30d":
                    list = (sortedBy != btn) ?
                        vm.CoinMarket.OrderByDescending(x => x.price_change_percentage_30d_in_currency).ToList() :
                        vm.CoinMarket.OrderBy(x => x.price_change_percentage_30d_in_currency).ToList();
                    break;
            }
            sortedBy = (sortedBy == btn) ? "" : btn;
            vm.CoinMarket = new ObservableCollection<CoinMarket>(list);
        }
    }
}
