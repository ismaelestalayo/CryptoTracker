using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Core.Constants;
using UWP.Helpers;
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

            var coinMarket = await LocalStorageHelper.ReadObject<List<CoinMarket>>(UserStorage.Top100List);
            vm.CoinMarket = new ObservableCollection<CoinMarket>(coinMarket);
            await Task.Delay(5000);
            var market = await Ioc.Default.GetService<ICoinGecko>().GetCoinsMarkets_("EUR");
            market = market.OrderBy(x => x.market_cap_rank).ToList();
            for (int i = 0; i < market.Count; i++) {
                vm.CoinMarket[i] = market[i];
            }

            
            LocalStorageHelper.SaveObject(UserStorage.Top100List, vm.CoinMarket);
            //tickers = tickers.OrderBy(x => x.rank).ToList();
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

    }
}
