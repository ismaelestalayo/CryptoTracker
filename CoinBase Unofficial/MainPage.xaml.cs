using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class MainPage : Page {

        private SolidColorBrush Color_CoinBase = new SolidColorBrush(Color.FromArgb(255, 0, 91, 148));
        private SolidColorBrush Color_CoinBaseButton = new SolidColorBrush(Color.FromArgb(255, 33, 132, 215));
        private SolidColorBrush Color_CoinBaseDark = new SolidColorBrush(Color.FromArgb(255, 0, 49, 80));

        public MainPage() {
            this.InitializeComponent();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView view = ApplicationView.GetForCurrentView();
            ApplicationViewTitleBar titleBar = view.TitleBar;

            titleBar.BackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            titleBar.ButtonBackgroundColor = Color.FromArgb(255, 34, 34, 34);  //New minimal gray
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
                statusBar.BackgroundColor = Color.FromArgb(255, 34, 34, 34);
                statusBar.BackgroundOpacity = 1;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }

            MainFrame.Navigate(typeof(Page_Home));
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Settings));
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

            } else if (x.Equals("CoinBase.Page_BTC")) {
                var p = (Page_BTC)MainFrame.Content;
                p.BTC_Update_click(null, null);

            } else if (x.Equals("CoinBase.Page_ETH")) {
                var p = (Page_ETH)MainFrame.Content;
                p.ETH_Update_click(null, null);

            } else if (x.Equals("CoinBase.Page_LTC")) {
                var p = (Page_LTC)MainFrame.Content;
                p.LTC_Update_click(null, null);
            }



        }

        private void HamburgerLogo_Click(object sender, RoutedEventArgs e) {
            //MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private void ShowVolumeChartButton_Click(object sender, RoutedEventArgs e) {
            string x = MainFrame.Content.ToString();

            if (x.Equals("CoinBase.Page_BTC")) {
                var p = (Page_BTC)MainFrame.Content;

                if (p.VolumeChart.Visibility == Visibility.Visible) {
                    p.VolumeChart.Visibility = Visibility.Collapsed;
                } else {
                    p.VolumeChart.Visibility = Visibility.Visible;
                }
            } else if (x.Equals("CoinBase.Page_ETH")) {
                var p = (Page_ETH)MainFrame.Content;

                if (p.VolumeChart.Visibility == Visibility.Visible) {
                    p.VolumeChart.Visibility = Visibility.Collapsed;
                } else {
                    p.VolumeChart.Visibility = Visibility.Visible;
                }
            } else if (x.Equals("CoinBase.Page_LTC")) {
                var p = (Page_LTC)MainFrame.Content;

                if (p.VolumeChart.Visibility == Visibility.Visible) {
                    p.VolumeChart.Visibility = Visibility.Collapsed;
                } else {
                    p.VolumeChart.Visibility = Visibility.Visible;
                }
            }
        }




    }
}
