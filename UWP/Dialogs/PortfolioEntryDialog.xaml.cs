using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.Models;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CryptoTracker.Dialogs {
    public sealed partial class PortfolioEntryDialog : ContentDialog {

        private Dictionary<string, string> AvailableCurrencies = Currencies.CurrencySymbol;

        public PortfolioEntryDialog() {
            InitializeComponent();

            RequestedTheme = ColorConstants.CurrentThemeIsDark() ? ElementTheme.Dark : ElementTheme.Light;
        }

        public static readonly DependencyProperty NewPurchaseProperty =
        DependencyProperty.Register(
            nameof(PurchaseModel),
            typeof(PurchaseModel),
            typeof(PortfolioEntryDialog),
            null);

        public PurchaseModel NewPurchase {
            get => (PurchaseModel)GetValue(NewPurchaseProperty);
            set => SetValue(NewPurchaseProperty, value);
        }

        private SuggestionCoin suggestionCoin = new SuggestionCoin();
        public SuggestionCoin SuggestionCoin {
            get => suggestionCoin;
            set {
                NewPurchase.Crypto = value?.Symbol;
                NewPurchase.CryptoName = value?.Name;
                suggestionCoin = value;
                AmountNumberBox.Focus(FocusState.Programmatic);
            }
        }

        /// ###############################################################################################
        private void PurchaseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(NewPurchase.Crypto) || NewPurchase.CryptoQty <= 0 || NewPurchase.InvestedQty < 0) {
                WarningMsg.Visibility = Visibility.Visible;
                args.Cancel = true;
            }
            else {
                //if (sender.PrimaryButtonText == "Add")
                //    vm.Portfolio.Add(vm.NewPurchase);
                //else if (sender.PrimaryButtonText == "Save") {
                //    vm.Portfolio.RemoveAt(EditingPurchaseId);
                //    vm.Portfolio.Insert(EditingPurchaseId, vm.NewPurchase);
                //}
            }
        }


        private async void DialogBtn_LostFocus(object sender, RoutedEventArgs e) {
            // If we change the crypto, set the current price to 0 so everything updates
            if (sender.GetType().Name == "ComboBox")
                NewPurchase.Current = 0;

            // If we have the coin and the quantity, we can update some properties
            if (!string.IsNullOrEmpty(NewPurchase.Crypto) && NewPurchase.CryptoQty > 0)
                NewPurchase = await UpdatePurchaseAsync(NewPurchase);
        }

        /// ###############################################################################################
        ///  Calculate a purchase's profit and worth live
        internal async Task<PurchaseModel> UpdatePurchaseAsync(PurchaseModel purchase)
            => await PortfolioHelper.UpdatePurchase(purchase);

    }
}
