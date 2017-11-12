using CoinBase.Helpers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class MainPage : Page {

        private SolidColorBrush Color_CoinBase = new SolidColorBrush(Color.FromArgb(255, 0, 91, 148));
        private SolidColorBrush Color_CoinBaseButton = new SolidColorBrush(Color.FromArgb(255, 33, 132, 215));
        private SolidColorBrush Color_CoinBaseDark = new SolidColorBrush(Color.FromArgb(255, 0, 49, 80));

        private bool isInSettings = false;
        private bool isInPortfolio = false;

        public MainPage() {
            this.InitializeComponent();

            // Clear the current tile
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop") {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                rootPivot.Padding = new Thickness(0, 30, 0, 0);
            }
            
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;

            //titleBar.BackgroundColor = Color.FromArgb(255, 242, 242, 242);
            //titleBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 242, 0, 242);
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 150, 150, 150);

            titleBar.InactiveBackgroundColor = Color.FromArgb(0, 242, 242, 242);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 242, 242, 242);
            
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();

                if(App.localSettings.Values["Theme"].Equals("Dark") ) {
                    statusBar.BackgroundColor = Color.FromArgb(255, 23, 23, 23);
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
                } else {
                    statusBar.BackgroundColor = Color.FromArgb(255, 242, 242, 242);
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                }

                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

                TopCommandBar.Visibility = Visibility.Collapsed;
                BottomCommandBar.Visibility = Visibility.Visible;
            }

            FirstRunDialogHelper.ShowIfAppropriateAsync();

            Frame0.Navigate(typeof(Page_Home));
            Frame1.Navigate(typeof(Page_BTC));
            Frame2.Navigate(typeof(Page_ETH));
            Frame3.Navigate(typeof(Page_LTC));
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }

        private void App_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false) {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        private void PortfolioButton_Click(object sender, RoutedEventArgs e) {

            if(!isInPortfolio) {
                rootPivot.SelectedIndex = 0;
                Frame0.Navigate(typeof(Page_Portfolio));
                isInPortfolio = true;
                isInSettings = false;
            } else {
                switch (rootPivot.SelectedIndex) {
                    case 0:
                        Frame0.Navigate(typeof(Page_Home));
                        break;
                    case 1:
                        Frame1.Navigate(typeof(Page_BTC));
                        break;
                    case 2:
                        Frame2.Navigate(typeof(Page_ETH));
                        break;
                    case 3:
                        Frame3.Navigate(typeof(Page_LTC));
                        break;
                }
                isInPortfolio = false;
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e){

            if (!isInSettings){
                rootPivot.SelectedIndex = 0;
                Frame0.Navigate(typeof(Page_Settings));
                isInSettings = true;
                isInPortfolio = false;
            }
            else{
                switch (rootPivot.SelectedIndex){
                    case 0:
                        Frame0.Navigate(typeof(Page_Home));
                        break;
                    case 1:
                        Frame1.Navigate(typeof(Page_BTC));
                        break;
                    case 2:
                        Frame2.Navigate(typeof(Page_ETH));
                        break;
                    case 3:
                        Frame3.Navigate(typeof(Page_LTC));
                        break;
                }
                isInSettings = false;
            }


        }

        internal async Task SyncAll() {
            var r = rootPivot;

            switch (rootPivot.SelectedIndex) {
                case 0:
                    if (!isInPortfolio) {
                        var p0 = (Page_Home)Frame0.Content;
                        p0.UpdateHome();
                    } else {
                        var p0 = (Page_Portfolio)Frame0.Content;
                        p0.UpdatePortfolio();
                    }
                    break;

                case 1:
                    var p1 = (Page_BTC)Frame1.Content;
                    p1.UpdatePage();
                    break;

                case 2:
                    var p2 = (Page_ETH)Frame2.Content;
                    p2.UpdatePage();
                    break;

                case 3:
                    var p3 = (Page_LTC)Frame3.Content;
                    p3.UpdatePage();
                    break;
            }

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }

        private void rootPivot_SelectionChanged(object sender, SelectionChangedEventArgs e){
            isInPortfolio = false;
            isInSettings = false;

            Frame0.Navigate(typeof(Page_Home));
        }
    }
}
