using UWP;
using UWP.Core.Constants;
using UWP.Views;
using Windows.UI;
using Windows.UI.ViewManagement;
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

        public SettingsDialog(string initialPage = "General") {
            this.InitializeComponent();

            var currentTheme = App._LocalSettings.Get<string>(UserSettings.Theme);
            switch (currentTheme) {
                case "Light":
                    this.RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    this.RequestedTheme = ElementTheme.Dark;
                    break;
                case "Windows":
                    this.RequestedTheme = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) ?
                        ElementTheme.Dark : ElementTheme.Light;
                    break;
            }

            NavigateFrame(initialPage);
        }

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

        private void ContentFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) {
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
                    this.SettingsFrame.Navigate(typeof(SettingsDialogGeneral));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[0];
                    break;
                case "Appearance":
                    this.SettingsFrame.Navigate(typeof(SettingsAppearance));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[1];
                    break;
                case "Tiles":
                    this.SettingsFrame.Navigate(typeof(SettingsAppearance));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[2];
                    break;
                case "Changelog":
                    this.SettingsFrame.Navigate(typeof(SettingsChangelog));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[3];
                    break;
                case "Feedback":
                    this.SettingsFrame.Navigate(typeof(SettingsFeedback));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[4];
                    break;
                case "About":
                    this.SettingsFrame.Navigate(typeof(SettingsAbout));
                    SettingsNavView.SelectedItem = SettingsNavView.MenuItems[5];
                    break;
            }
        }
    }
}
