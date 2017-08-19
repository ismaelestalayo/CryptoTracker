using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GDAX {
    public sealed partial class MainPage : Page {

        private bool LightTheme = true;
        private string content = "&#xE706;";

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
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(0, 20, 20, 20);
            titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0, 50, 50, 50);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 0, 91, 148);
            titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0, 255, 255, 255);      

            MainFrame.Navigate(typeof(Page2));
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e) { 

            string x = MainFrame.Content.ToString();
            if (x.Equals("GDAX.Page2"))
                MainFrame.Navigate(typeof(SettingsPage));

            else if (x.Equals("GDAX.SettingsPage"))
                MainFrame.GoBack();
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e) {
            if (LightTheme) { 
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                LightTheme = false;
            } else {
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                LightTheme = true;
            }

        }

        private void SyncAllButton_Click(object sender, RoutedEventArgs e) {
            SyncAll();
        }
        private async Task SyncAll() {
            var p = (Page2)MainFrame.Content;
            p.BTC_Update_click(null, null);
            p.ETH_Update_click(null, null);
            p.LTC_Update_click(null, null);

        }
    }
}
