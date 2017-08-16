using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace GDAX {
    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationView view = ApplicationView.GetForCurrentView();
            ApplicationViewTitleBar titleBar = view.TitleBar;

            titleBar.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
            titleBar.ForegroundColor = Color.FromArgb(0, 0, 0, 0);
            titleBar.InactiveBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            titleBar.InactiveForegroundColor = Color.FromArgb(0, 0, 0, 0);

            titleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(100, 10, 10, 10);
            //titleBar.ButtonPressedBackgroundColor = Color.FromArgb(0, 20, 20, 20);
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 255, 0, 255);
            titleBar.ButtonInactiveForegroundColor = Color.FromArgb(0, 0, 0, 0);

            MainFrame.Navigate(typeof(Page2));
        }

        private void SettingsButton(object sender, RoutedEventArgs e) { 

            string x = MainFrame.Content.ToString();
            if (x.Equals("GDAX.Page2"))
                MainFrame.Navigate(typeof(SettingsPage));

            else if (x.Equals("GDAX.SettingsPage"))
                MainFrame.GoBack();
        }
    }
}
