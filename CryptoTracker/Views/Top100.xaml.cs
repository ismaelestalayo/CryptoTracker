using CryptoTracker.APIs;
using CryptoTracker.Models;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {

    public sealed partial class Top100 : Page {

        public Top100() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            await InitPage();
        }

        // #########################################################################################
        private async Task InitPage() {
            vm.GlobalStats = await CoinGecko.GetGlobalStats();
            vm.Top100cards = await CryptoCompare.GetTop100();
        }

        // #########################################################################################
        private void ListView_Click(object sender, ItemClickEventArgs e) {
            top100ListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "listView_Element");

            var crypto = ((Top100card)e.ClickedItem).CoinInfo.Name;
            this.Frame.Navigate(typeof(CoinDetails), crypto);
        }

        private void Pin_click(object sender, RoutedEventArgs e) {
            var card = ((Top100card)((FrameworkElement)sender).DataContext);
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
    }
}
