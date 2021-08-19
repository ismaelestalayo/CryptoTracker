using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsGeneral : Page {

        private Dictionary<string, string> AvailableCurrencies = Currencies.CurrencySymbol;

        public SettingsGeneral() {
            InitializeComponent();
            Loaded += SettingsGeneral_Loaded;
        }

        private void SettingsGeneral_Loaded(object sender, RoutedEventArgs e) {
            vm.AutoRefresh = App._LocalSettings.Get<string>(UserSettings.AutoRefresh);
            vm.Currency = App._LocalSettings.Get<string>(UserSettings.Currency);
            startupPage.PlaceholderText = App._LocalSettings.Get<string>(UserSettings.StartupPage).Replace("/", "");
        }

        // ###############################################################################################
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
            //        LocalStorageHelper.SaveObject(PortfolioKey, obj);
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

        private void startupPage_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var startupPage = ((ComboBox)sender).SelectedItem.ToString();
            App._LocalSettings.Set(UserSettings.StartupPage, $"/{startupPage}");
        }
    }
}
