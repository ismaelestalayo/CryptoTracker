using System;
using System.Threading.Tasks;
using CoinBase.Helpers;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
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

            // Clear the current tile
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView view = ApplicationView.GetForCurrentView();
            ApplicationViewTitleBar titleBar = view.TitleBar;

            /// Alpha channel does nothing 
            /// (guess it's not supported on TitleBars
            titleBar.BackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
            titleBar.ButtonBackgroundColor = Color.FromArgb(255, 34, 34, 34);  //New minimal gray
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 255, 255, 255);

            titleBar.InactiveBackgroundColor = Color.FromArgb(255, 0, 91, 148);
            titleBar.InactiveForegroundColor = Color.FromArgb(255, 255, 255, 255);

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

            FirstRunDialogHelper.ShowIfAppropriateAsync();
            MainFrame.Navigate(typeof(Page_Home));
            //SyncAll();
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
        private void MenuAll_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_All));
        }
        private void MenuPortfolio_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Portfolio));
        }
        private void MenuSettings_Click(object sender, RoutedEventArgs e) {
            MainFrame.Navigate(typeof(Page_Settings));
        }

        private void HamburgerLogo_Click(object sender, RoutedEventArgs e) {
            //MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();
            //if (r == 0) {
            //    await UpdateIcon.Rotate(value: 360,
            //                 centerX: 9,
            //                 centerY: 9,
            //                 duration: 1000, delay: 0).StartAsync();
            //    r = 1;
            //} else {
            //    await UpdateIcon.Rotate(value: 0,
            //                 centerX: 9,
            //                 centerY: 9,
            //                 duration: 1000, delay: 0).StartAsync();
            //    r = 0;
            //}

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }

        private async Task SyncAll() {
            string x = MainFrame.Content.ToString();

            if (x.Equals("CoinBase.Page_Home")) {
                var p = (Page_Home)MainFrame.Content;
                p.BTC_Update_click();
                p.ETH_Update_click();
                p.LTC_Update_click();

            } else if (x.Equals("CoinBase.Page_BTC")) {
                var p = (Page_BTC)MainFrame.Content;
                p.BTC_Update_click();

            } else if (x.Equals("CoinBase.Page_ETH")) {
                var p = (Page_ETH)MainFrame.Content;
                p.ETH_Update_click();

            } else if (x.Equals("CoinBase.Page_LTC")) {
                var p = (Page_LTC)MainFrame.Content;
                p.LTC_Update_click();
            } else if (x.Equals("CoinBase.Page_Portfolio")) {
                var p = (Page_Portfolio)MainFrame.Content;
                p.UpdatePortfolio();
            }

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }


        

    }
}
