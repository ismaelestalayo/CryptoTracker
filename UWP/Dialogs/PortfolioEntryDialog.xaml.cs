using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Dialogs {
    public sealed partial class PortfolioEntryDialog : ContentDialog {

        private Dictionary<string, string> AvailableCurrencies = Currencies.CurrencySymbol;

        public PortfolioEntryDialog() {
            InitializeComponent();
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
        internal async Task<PurchaseModel> UpdatePurchaseAsync(PurchaseModel purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = await Ioc.Default.GetService<ICryptoCompare>().GetPrice_Extension(
                    crypto, purchase.Currency);

            var curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            /// If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty >= 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                purchase.BoughtAt = priceBought;
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(earningz, 2);
                purchase.ProfitFG = (earningz < 0) ?
                    (SolidColorBrush)App.Current.Resources["pastelRed"] :
                    (SolidColorBrush)App.Current.Resources["pastelGreen"];
            }
            if (purchase.InvestedQty == 0)
                purchase.Delta = 0;

            return purchase;
        }
    }
}
