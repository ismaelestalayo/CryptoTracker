using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GDAX {
    public sealed partial class MainPage : Page {

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
    }
}
