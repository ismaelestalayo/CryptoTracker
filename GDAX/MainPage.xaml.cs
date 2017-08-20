using CoinBase;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CoinBase {
    public sealed partial class MainPage : Page {

        private bool LightTheme = true;

        public MainPage() {
            this.InitializeComponent();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            ApplicationView view = ApplicationView.GetForCurrentView();
            ApplicationViewTitleBar titleBar = view.TitleBar;

            titleBar.BackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);

            titleBar.InactiveBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            titleBar.InactiveForegroundColor = Color.FromArgb(0, 255, 255, 255);

            /// Alpha channel does nothing 
            /// (guess it's not supported on TitleBars

            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            titleBar.ButtonForegroundColor = Color.FromArgb(0, 255, 255, 255);
            //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0, 20, 20, 20);
            //titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0, 50, 50, 50);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            //titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0, 255, 255, 255);

            MainFrame.Navigate(typeof(Page_Home));
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Settings));
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
        }

        private void MenuBTC_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_BTC));
        }

        private void MenuETH_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_ETH));
        }

        private void MenuLTC_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_LTC));
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
