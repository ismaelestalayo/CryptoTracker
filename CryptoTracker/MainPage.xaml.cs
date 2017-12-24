using CryptoTracker.Helpers;
using Microsoft.AppCenter.Analytics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CryptoTracker {
    public sealed partial class MainPage : Page {

        private bool isInSettings = false;
        private bool isInPortfolio = false;

        public MainPage() {
            this.InitializeComponent();

            // Clear the current tile
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop") {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                TopCommandBar.Visibility = Visibility.Visible;
            }

            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            //Transparent buttons, with gray foreground
            titleBar.ButtonBackgroundColor         = Color.FromArgb(0, 242, 0, 242);
            titleBar.ButtonForegroundColor         = Color.FromArgb(255, 150, 150, 150);
            titleBar.InactiveBackgroundColor       = Color.FromArgb(0, 242, 0, 242);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 242, 0, 242);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();

                if (App.localSettings.Values["Theme"].Equals("Dark")) {
                    statusBar.BackgroundColor = Color.FromArgb(255, 23, 23, 23);
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
                } else {
                    statusBar.BackgroundColor = Color.FromArgb(255, 242, 242, 242);
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                }

                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
                BottomCommandBar.Visibility = Visibility.Visible;
            }

            FirstRunDialogHelper.ShowIfAppropriateAsync();
            rootFrame.Navigate(typeof(Page_Home));
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }

        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e) {
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not already been handled .
            if (rootFrame.CanGoBack && e.Handled == false) {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }


        private void PortfolioButton_Click(object sender, RoutedEventArgs e) {
            if(rootFrame.SourcePageType.Name != "Page_Portfolio") {
                rootFrame.Navigate(typeof(Page_Portfolio));
                Analytics.TrackEvent("Section_Portflio");
            } else {
                rootFrame.Navigate(typeof(Page_Home));
            }
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e) {
            if (rootFrame.SourcePageType.Name != "Page_Settings") {
                rootFrame.Navigate(typeof(Page_Settings));
                Analytics.TrackEvent("Section_Settings");
            } else {
                rootFrame.Navigate(typeof(Page_Home));
            }
        }
        private void News_Click(object sender, RoutedEventArgs e) {

        }


        internal async Task SyncAll() {

            switch (rootFrame.SourcePageType.Name) {
                case "Page_CoinTemplate":
                    var p0 = (Page_CoinTemplate)rootFrame.Content;
                    p0.UpdatePage();
                    break;
                case "Page_BTC":
                    var p2 = (Page_CoinTemplate)rootFrame.Content;
                    p2.UpdatePage();
                    break;
                case "Page_Portfolio":
                    var p1 = (Page_Portfolio)rootFrame.Content;
                    p1.UpdatePortfolio();
                    break;
            }

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }
    }
}
