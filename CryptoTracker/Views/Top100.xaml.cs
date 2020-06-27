using CryptoTracker.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Views {

    public class GlobalStats {
        public string Currency { get; set; }
        public string TotalMarketCap { get; set; }
        public string TotalVolume { get; set; }
        public string BtcDominance { get; set; }
        public string ActiveCurrencies { get; set; }
    }

    public class Top100coin : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaiseProperty(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Rank { get; set; }
        public string Price { get; set; }
        public string Vol24 { get; set; }
        public string MarketCap { get; set; }
        public string Change24h { get; set; }
        public SolidColorBrush ChangeFG { get; set; }
        public string Src { get; set; }
        public string LogoURL { get; set; }
        private string _favIcon;
        public string FavIcon {
            get { return _favIcon; }
            set {
                _favIcon = value;
                RaiseProperty(nameof(FavIcon));
            }
        }

        public string curr;
        public string _priceCurr {
            get { return curr; }
            set {
                curr = value;
                RaiseProperty(nameof(_priceCurr));
            }
        }
    }

    // #########################################################################################
    // #########################################################################################
    // #########################################################################################
    public sealed partial class Top100 : Page {

        private GlobalStats g;
        private ObservableCollection<Top100coin> topCoins { get; set; }

        public Top100() {
            this.InitializeComponent();

            InitPage();
        }

        // #########################################################################################
        private async void InitPage() {
            g = await App.GetGlobalStats();
            DataContext = g;

            topCoins = await App.GetTop100();
            
            for (int i = 0; i < topCoins.Count; i++) {
                topCoins[i].LogoURL = IconsHelper.GetIcon(topCoins[i].Symbol);
            }

            top100ListView.ItemsSource = topCoins;
        }

        // #########################################################################################
        private void ListView_Click(object sender, ItemClickEventArgs e) {
            top100ListView.PrepareConnectedAnimation("toCoinDetails", e.ClickedItem, "listView_Element");

            var clickedItem = (Top100coin)e.ClickedItem;
            this.Frame.Navigate(typeof(CoinDetails), clickedItem.Symbol);
        }

        private void Pin_click(object sender, RoutedEventArgs e) {
            string c = ((Top100coin)((FrameworkElement)((FrameworkElement)sender).Parent).DataContext).Symbol;
            if (!App.pinnedCoins.Contains(c)) {
                App.pinnedCoins.Add(c);
                Home.AddCoinHome(c);
                inAppNotification.Show(c + " pinned to home.", 2000);
            } else {
                Home.RemoveCoinHome(c);
                inAppNotification.Show(c + " unpinned from home.", 2000);
            }

            int index = int.Parse(((Top100coin)((FrameworkElement)((FrameworkElement)sender).Parent).DataContext).Rank) - 1;

            // TODO: deleting from Home should empty a heart icon
            topCoins[index].FavIcon = topCoins[index].FavIcon.Equals("\uEB51") ? "\uEB52" : "\uEB51";

            // Update pinnedCoin list
            App.UpdatePinnedCoins();
        }
    }
}
