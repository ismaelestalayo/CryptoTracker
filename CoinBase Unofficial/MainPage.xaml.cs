using CoinBase;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class MainPage : Page {

        private bool LightTheme = true;

        private SolidColorBrush Color_CoinBase       = new SolidColorBrush(Color.FromArgb(255, 0,   91, 148));
        private SolidColorBrush Color_CoinBaseButton = new SolidColorBrush(Color.FromArgb(255, 33, 132, 215));
        private SolidColorBrush Color_CoinBaseDark = new SolidColorBrush(Color.FromArgb(255, 0,   49,  80));

        public MainPage() {
            this.InitializeComponent();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            ApplicationView view = ApplicationView.GetForCurrentView();
            ApplicationViewTitleBar titleBar = view.TitleBar;

            titleBar.BackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            titleBar.ButtonBackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 255, 255, 255);

            titleBar.InactiveBackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.InactiveForegroundColor = Color.FromArgb(255, 255, 255, 255);

            /// Alpha channel does nothing 
            /// (guess it's not supported on TitleBars
            
            //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0, 20, 20, 20);
            //titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0, 50, 50, 50);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            //titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0, 255, 255, 255);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Color.FromArgb(255, 0, 91, 148);
                statusBar.BackgroundOpacity = 1;
            }

            MainFrame.Navigate(typeof(Page_Home));
            MenuHome.Background = Color_CoinBaseDark;
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Settings));
            MenuHome.Background= Color_CoinBase;
            MenuBTC.Background = Color_CoinBase;
            MenuETH.Background = Color_CoinBase;
            MenuLTC.Background = Color_CoinBase;
            MenuSettings.Background = Color_CoinBaseDark;
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e) {
            if (LightTheme) {
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                LightTheme = false;
            }
            else {
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                LightTheme = true;
            }

        }

        private void MenuHome_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Home));
            MenuHome.Background= Color_CoinBaseDark;
            MenuBTC.Background = Color_CoinBase;
            MenuETH.Background = Color_CoinBase;
            MenuLTC.Background = Color_CoinBase;
            MenuSettings.Background = Color_CoinBase;
        }

        private void MenuBTC_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_BTC));
            MenuHome.Background= Color_CoinBase;
            MenuBTC.Background = Color_CoinBaseDark;
            MenuETH.Background = Color_CoinBase;
            MenuLTC.Background = Color_CoinBase;
            MenuSettings.Background = Color_CoinBase;
        }

        private void MenuETH_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_ETH));
            MenuHome.Background = Color_CoinBase;
            MenuBTC.Background = Color_CoinBase;
            MenuETH.Background = Color_CoinBaseDark;
            MenuLTC.Background = Color_CoinBase;
            MenuSettings.Background = Color_CoinBase;
        }

        private void MenuLTC_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_LTC));
            MenuHome.Background = Color_CoinBase;
            MenuBTC.Background = Color_CoinBase;
            MenuETH.Background = Color_CoinBase;
            MenuLTC.Background = Color_CoinBaseDark;
            MenuSettings.Background = Color_CoinBase;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        private void SyncAllButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();
        }

        private async Task SyncAll() {
            string x = MainFrame.Content.ToString();

            if (x.Equals("CoinBase.Page_Home")) {
                var p = (Page_Home)MainFrame.Content;
                p.BTC_Update_click(null, null);
                p.ETH_Update_click(null, null);
                p.LTC_Update_click(null, null);
            }
            else if (x.Equals("CoinBase.Page_BTC")) {
                var p = (Page_BTC)MainFrame.Content;
                p.BTC_Update_click(null, null);
            }
            else if (x.Equals("CoinBase.Page_ETH")) {
                var p = (Page_ETH)MainFrame.Content;
                p.ETH_Update_click(null, null);
            }
            else if (x.Equals("CoinBase.Page_LTC")) {
                var p = (Page_LTC)MainFrame.Content;
                p.LTC_Update_click(null, null);
            }



        }

        private void HamburgerLogo_Click(object sender, RoutedEventArgs e) {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
    }
}
