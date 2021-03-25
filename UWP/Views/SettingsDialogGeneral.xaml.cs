using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsDialogGeneral : Page {

        private PackageVersion version;
        private string PortfolioKey = "Portfolio";

        public SettingsDialogGeneral() {
            this.InitializeComponent();            

            
            version = Package.Current.Id.Version;

            ThemeComboBox.PlaceholderText = App._LocalSettings.Get<string>(UserSettings.Theme);

            var currency = App._LocalSettings.Get<string>(UserSettings.Currency);
            switch (currency) {
                case "EUR":
                    EUR.IsSelected = true;
                    break;
                case "USD":
                    USD.IsSelected = true;
                    break;
                case "GBP":
                    GBP.IsSelected = true;
                    break;
                case "CAD":
                    CAD.IsSelected = true;
                    break;
                case "AUD":
                    AUD.IsSelected = true;
                    break;
                case "MXN":
                    MXN.IsSelected = true;
                    break;
                case "CNY":
                    CNY.IsSelected = true;
                    break;
                case "JPY":
                    JPY.IsSelected = true;
                    break;
                case "INR":
                    INR.IsSelected = true;
                    break;
            }
            CoinComboBox.PlaceholderText = currency;

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

        private async void UploadPortfolio_Click(object sender, RoutedEventArgs e) {            
            //try {
            //    var helper = new RoamingObjectStorageHelper();
            //    var portfolio = await LocalStorageHelper.ReadObject<ObservableCollection<PurchaseModel>>(PortfolioKey);
                
            //    if (portfolio == null || portfolio.Count == 0) {
            //        await new ContentDialog() {
            //            Title = "Empty portfolio",
            //            Content = "Your current portfolio is empty. Add a purchase before uploading it to the cloud.",
            //            DefaultButton = ContentDialogButton.Primary,
            //            PrimaryButtonText = "Export",
            //            IsPrimaryButtonEnabled = false,
            //            CloseButtonText = "Cancel",
            //            RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
            //        }.ShowAsync();
            //    }

            //    else {
            //        var response = await new ContentDialog() {
            //            Title = $"Export {portfolio.Count} purchases?",
            //            Content = "This will create a backup of your current portfolio in the cloud.",
            //            DefaultButton = ContentDialogButton.Primary,
            //            PrimaryButtonText = "Export",
            //            CloseButtonText = "Cancel",
            //            RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
            //        }.ShowAsync();

            //        if (response == ContentDialogResult.Primary)
            //            await helper.SaveFileAsync(PortfolioKey, portfolio);
            //    }
            //}
            //catch (Exception ex) {
            //    await new MessageDialog("Error uploading your portfolio. Try again later.").ShowAsync();
            //    await new ContentDialog() {
            //        Title = "Error uploading your portfolio.",
            //        Content = $"Please, try again later. Error: {ex}",
            //        DefaultButton = ContentDialogButton.Close,
            //        PrimaryButtonText = "Export",
            //        IsPrimaryButtonEnabled = false,
            //        CloseButtonText = "Cancel",
            //        RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
            //    }.ShowAsync();
            //}
        }

        private async void DownloadPortfolio_Click(object sender, RoutedEventArgs e) {
            //var helper = new RoamingObjectStorageHelper();
            
            //if (await helper.FileExistsAsync(PortfolioKey)) {
            //    var obj = await helper.ReadFileAsync<ObservableCollection<PurchaseModel>>(PortfolioKey);

            //    var response = await new ContentDialog() {
            //        Title = $"Import {obj.Count} purchases?",
            //        Content = "This will clear your current portfolio and download your backup.",
            //        DefaultButton = ContentDialogButton.Primary,
            //        PrimaryButtonText = "Import",
            //        CloseButtonText = "Cancel",
            //        RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
            //    }.ShowAsync();

            //    if (response == ContentDialogResult.Primary) {
            //        LocalStorageHelper.SaveObject(obj, PortfolioKey);
            //        vm.InAppNotification("Portfolio imported succesfully.");
            //    }
            //}
            //else {
            //    await new ContentDialog() {
            //        Title = "No backup found.",
            //        Content = "You don't seem to have uploaded any portfolio before.",
            //        DefaultButton = ContentDialogButton.Primary,
            //        IsPrimaryButtonEnabled = false,
            //        PrimaryButtonText = "Import",
            //        CloseButtonText = "Cancel",
            //        RequestedTheme = ((Frame)Window.Current.Content).RequestedTheme
            //    }.ShowAsync();
            //}
        }

        private void ThemeComboBox_changed(object sender, SelectionChangedEventArgs e) {
            ComboBox c = sender as ComboBox;
            var theme = ((ComboBoxItem)c.SelectedItem).Name.ToString();

            App._LocalSettings.Set(UserSettings.Theme, theme);
            switch (theme) {
                case "Light":
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
                case "Dark":
                    ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    break;
                case "Windows":
                    if (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black)
                        ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                    else
                        ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                    break;
            }
        }
        
    }
}
