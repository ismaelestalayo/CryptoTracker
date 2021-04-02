using UWP;
using UWP.Core.Constants;
using UWP.Views;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
    public sealed partial class SettingsDialog : ContentDialog {
        /// Store a refrence to Rectangle to later unregester the event handler
        private Rectangle _lockRectangle;

        public SettingsDialog(string initialPage = "General") {
            InitializeComponent();

            var currentTheme = App._LocalSettings.Get<string>(UserSettings.Theme);
            switch (currentTheme) {
                case "Light":
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    RequestedTheme = ElementTheme.Dark;
                    break;
                default:
                case "Windows":
                    RequestedTheme = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) ?
                        ElementTheme.Dark : ElementTheme.Light;
                    break;
            }

            var version = Package.Current.Id.Version;
            FooterVersion.Content = string.Format("CryptoTracker {0}.{1}.{2}",
                version.Major, version.Minor, version.Build);
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
        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e) {
            var z = sender;
            var toPage = (e.SourcePageType).Name;
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
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
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[1];
                    break;
                case "Alerts":
                    SettingsFrame.Navigate(typeof(SettingsAlerts));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[2];
                    break;
                case "Changelog":
                    SettingsFrame.Navigate(typeof(SettingsChangelog));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[3];
                    break;
                case "Feedback":
                    SettingsFrame.Navigate(typeof(SettingsFeedback));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[4];
                    break;
                case "About":
                    SettingsFrame.Navigate(typeof(SettingsAbout));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[5];
                    break;
            }
        }
    }
}
