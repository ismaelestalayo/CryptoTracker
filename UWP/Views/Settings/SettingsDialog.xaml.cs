using UWP.Shared.Constants;
using UWP.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
    public sealed partial class SettingsDialog : ContentDialog {

        public SettingsDialog(string initialPage = "General") {
            InitializeComponent();

            RequestedTheme = ColorConstants.CurrentThemeIsDark() ? ElementTheme.Dark : ElementTheme.Light;

            NavigateFrame(initialPage);
        }

        /// #######################################################################################
        private void NavView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args) {
            var selected = ((ContentControl)args.SelectedItem).Content.ToString();
            NavigateFrame(selected);
        }

        private void NavigateFrame(string page) {
            switch (page) {
                default:
                case "General":
                    SettingsFrame.Navigate(typeof(SettingsGeneral));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[0];
                    break;
                case "Appearance":
                    SettingsFrame.Navigate(typeof(SettingsAppearance));
                    break;
                case "Alerts":
                    SettingsFrame.Navigate(typeof(SettingsAlerts));
                    break;
                case "Changelog":
                    SettingsFrame.Navigate(typeof(SettingsChangelog));
                    break;
                case "Feedback":
                    SettingsFrame.Navigate(typeof(SettingsFeedback));
                    break;
                case "About":
                    SettingsFrame.Navigate(typeof(SettingsAbout));
                    break;
                case "Calculator":
                    SettingsFrame.Navigate(typeof(SettingsCalculator));
                    break;
            }
        }

        private void NavViewClose_Tapped(object sender, TappedRoutedEventArgs e)
            => Hide();
    }
}
