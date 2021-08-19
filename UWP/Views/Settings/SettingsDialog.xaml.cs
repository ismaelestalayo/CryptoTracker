using UWP.Shared.Constants;
using UWP.Views;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
    public sealed partial class SettingsDialog : ContentDialog {
        /// Store a refrence to Rectangle to later unregester the event handler
        private Rectangle _lockRectangle;

        private string FooterVersion;

        public SettingsDialog(string initialPage = "General") {
            InitializeComponent();

            RequestedTheme = ColorConstants.CurrentThemeIsDark() ? ElementTheme.Dark : ElementTheme.Light;

            var version = Package.Current.Id.Version;
            FooterVersion = string.Format("CryptoTracker {0}.{1}.{2}", version.Major, version.Minor, version.Build);
            NavigateFrame(initialPage);
        }

        /// #######################################################################################
        /// Light dismiss functionality
        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // get all open popups
            // normally there are 2 popups, one for your ContentDialog and one for Rectangle
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups) {
                if (popup.Child is Rectangle) {
                    _lockRectangle = popup.Child as Rectangle;
                    _lockRectangle.Tapped += OnLockRectangleTapped;
                }
            }
        }

        private void OnLockRectangleTapped(object sender, TappedRoutedEventArgs e) {
            this.Hide();
            _lockRectangle.Tapped -= OnLockRectangleTapped;
        }

        /// #######################################################################################
        private void NavView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args) {
            string source;
            
            if (((Frame)sender.Content).SourcePageType != null)
                source = (((Frame)sender.Content).SourcePageType).Name;

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

        private void CloseDialog_Click(object sender, RoutedEventArgs e) {
            this.Hide();
        }
    }
}
