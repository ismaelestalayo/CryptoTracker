using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAbout : Page {

        private PackageVersion version;
        private string PortfolioKey = "Portfolio";

        public SettingsAbout() {
            this.InitializeComponent();
            version = Package.Current.Id.Version;

            ThemeComboBox.PlaceholderText = App._LocalSettings.Get<string>(UserSettings.Theme);

        }


        // ###############################################################################################
        private void CoinBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            var currency = ((ComboBoxItem)c.SelectedItem).Name.ToString();
            var currencySym = Currencies.GetCurrencySymbol(currency);

            App._LocalSettings.Set(UserSettings.Currency, currency);
            App._LocalSettings.Set(UserSettings.CurrencySymbol, currencySym);
            
            App.currency = currency;
            App.currencySymbol = currencySym;
        }

        private void ThemeComboBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            var theme = ((ComboBoxItem)c.SelectedItem).Name.ToString();

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
