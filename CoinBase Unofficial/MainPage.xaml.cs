using CoinBase.Helpers;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CoinBase
{
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
            titleBar.BackgroundColor = Color.FromArgb(0, 0, 91, 148);
            titleBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 34, 34, 34);  //New minimal gray
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 0, 0, 0);

            titleBar.InactiveBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            titleBar.InactiveForegroundColor = Color.FromArgb(255, 0, 0, 0);

            //titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0, 20, 20, 20);
            //titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0, 50, 50, 50);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            //titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0, 255, 255, 255);

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = Color.FromArgb(255, 242, 242, 242);
                statusBar.BackgroundOpacity = 1;
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }

            FirstRunDialogHelper.ShowIfAppropriateAsync();

            Frame0.Navigate(typeof(Page_Home));
            Frame1.Navigate(typeof(Page_BTC));
            Frame2.Navigate(typeof(Page_ETH));
            Frame3.Navigate(typeof(Page_LTC));
            FrameSettings.Navigate(typeof(Page_Settings));
            //SyncAll();
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        private void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e){
            
        }

        private async Task SyncAll() {
            //string x = MainFrame.Content.ToString();

            //if (x.Equals("CoinBase.Page_Home")) {
            //    var p = (Page_Home)MainFrame.Content;
            //    p.UpdateHome();

            //} else if (x.Equals("CoinBase.Page_BTC")) {
            //    var p = (Page_BTC)MainFrame.Content;
            //    p.BTC_Update_click();

            //} else if (x.Equals("CoinBase.Page_ETH")) {
            //    var p = (Page_ETH)MainFrame.Content;
            //    p.ETH_Update_click();

            //} else if (x.Equals("CoinBase.Page_LTC")) {
            //    var p = (Page_LTC)MainFrame.Content;
            //    p.LTC_Update_click();
            //} else if (x.Equals("CoinBase.Page_Portfolio")) {
            //    var p = (Page_Portfolio)MainFrame.Content;
            //    p.UpdatePortfolio();
            //}

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }


        

    }
}
