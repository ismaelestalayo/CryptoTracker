using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml.Media;

namespace CoinBase {
    public sealed partial class MainPage : Page {

        private int r = 0;
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

        ////////////////////////////////////////////////////////////////////////////////////////
        private async void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();
            if (r == 0) {
                await UpdateIcon.Rotate(value: 360,
                             centerX: 9,
                             centerY: 9,
                             duration: 1000, delay: 0).StartAsync();
                r = 1;
            } else {
                await UpdateIcon.Rotate(value: 0,
                             centerX: 9,
                             centerY: 9,
                             duration: 1000, delay: 0).StartAsync();
                r = 0;
            }



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

        private async void LiveTileButton_Click(object sender, RoutedEventArgs e) {
            testLiveTile();


        }







        public void testLiveTile() {

            var tileContent = new TileContent() {
                Visual = new TileVisual() {
                    Branding = TileBranding.Name,

                    TileMedium = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText(){
                                    Text = "9:50 AM, Wednesday",
                                    HintStyle = AdaptiveTextStyle.Caption
                                },
                                new AdaptiveText(){
                                    Text = "263 Grove St, San Francisco, CA 94102",
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                    HintWrap = true
                                }
                            }
                        }
                    },

                    TileWide = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children ={
                                new AdaptiveGroup(){
                                    Children ={
                                        new AdaptiveSubgroup(){
                                            HintWeight = 33,
                                            Children ={
                                                new AdaptiveImage(){
                                                    Source = "Assets/coinbase.png"
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup(){
                                            Children ={
                                                new AdaptiveText(){
                                                    Text = "9:50 AM, Wednesday",
                                                    HintStyle = AdaptiveTextStyle.Caption
                                                },
                                                new AdaptiveText(){
                                                    Text = "263 Grove St, San Francisco, CA 94102",
                                                    HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                                    HintWrap = true,
                                                    HintMaxLines = 3
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    TileLarge = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveGroup(){
                                    Children ={
                                        new AdaptiveSubgroup(){
                                            HintWeight = 33,
                                            Children = {
                                                new AdaptiveImage(){
                                                    Source = "/Assets/coinbase.png"
                                                }
                                            }
                                        },
                                        new AdaptiveSubgroup(){
                                            Children = {
                                                new AdaptiveText(){
                                                    Text = "9:50 AM, Wednesday",
                                                    HintStyle = AdaptiveTextStyle.Caption
                                                }
                                            }
                                        }
                                    }
                                },
                                new AdaptiveImage(){
                                    Source = "Assets/coinbase.png"
                                }
                            }
                        }
                    }
                }
            };

            // Create the tile notification
            var tileNotif = new TileNotification(tileContent.GetXml());

            // And send the notification to the primary tile
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotif);
        }

    }
}
