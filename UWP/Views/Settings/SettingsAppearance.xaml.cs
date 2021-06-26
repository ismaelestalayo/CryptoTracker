using Microsoft.UI.Xaml.Controls;
using UWP.Core.Constants;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAppearance : Page {

        public SettingsAppearance() {
            this.InitializeComponent();

            var theme = App._LocalSettings.Get<string>(UserSettings.Theme);
            switch (theme) {
                case "Light":
                    ThemeRadioButtons.SelectedIndex = 0;
                    break;
                case "Dark":
                    ThemeRadioButtons.SelectedIndex = 1;
                    break;
                default:
                case "Windows":
                    ThemeRadioButtons.SelectedIndex = 2;
                    break;
            }
        }


        // ###############################################################################################
        private void ThemeRadioButtons_Changed(object sender, SelectionChangedEventArgs e) {
            RadioButtons r = sender as RadioButtons;
            if (r.SelectedIndex < 0)
                return;
            var theme = ((ContentControl)r.SelectedItem).Content.ToString();

            var parentFrame = (Frame)Window.Current.Content;
            var parentDialog = (FrameworkElement)((FrameworkElement)((FrameworkElement)this.Parent).Parent).Parent;

            App._LocalSettings.Set(UserSettings.Theme, theme);
            switch (theme) {
                case "Light":
                    parentFrame.RequestedTheme = ElementTheme.Light;
                    parentDialog.RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    parentFrame.RequestedTheme = ElementTheme.Dark;
                    parentDialog.RequestedTheme = ElementTheme.Dark;
                    break;
                case "Windows":
                    bool isDark = new UISettings().GetColorValue(UIColorType.Background) == Colors.Black;
                    parentFrame.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light;
                    parentDialog.RequestedTheme = isDark ? ElementTheme.Dark : ElementTheme.Light;
                    break;
            }
        }
    }
}
