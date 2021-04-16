using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWP;
using UWP.APIs;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Dialogs {
    public sealed partial class PortfolioEntryDialog : ContentDialog {
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

        private void PurchaseDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
            if (string.IsNullOrEmpty(NewPurchase.Crypto) || NewPurchase.CryptoQty <= 0 || NewPurchase.InvestedQty < 0) {
                args.Cancel = true;
                new MessageDialog("You must fill Crypto, Amount and Invested fields.").ShowAsync();
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

        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e) {
            AutoSuggestBox box = sender as AutoSuggestBox;
            box.ItemsSource = FilterCoins(box);
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
            => CoinAutoSuggestBox.Text = ((SuggestionCoin)args.SelectedItem).Symbol;

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            NewPurchase.Crypto = ((SuggestionCoin)args.ChosenSuggestion)?.Symbol;
            NewPurchase.CryptoName = ((SuggestionCoin)args.ChosenSuggestion)?.Name;
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                sender.ItemsSource = FilterCoins(sender);
        }

        private List<SuggestionCoin> FilterCoins(AutoSuggestBox box) {
            var filtered = App.coinList.Where(x =>
                x.symbol.Contains(box.Text, StringComparison.InvariantCultureIgnoreCase) ||
                x.name.Contains(box.Text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            List<SuggestionCoin> list = new List<SuggestionCoin>();
            foreach (CoinBasicInfo coin in filtered) {
                list.Add(new SuggestionCoin {
                    Icon = IconsHelper.GetIcon(coin.symbol),
                    Name = coin.name,
                    Symbol = coin.symbol
                });
            }
            return list;
        }

        /// ###############################################################################################
        ///  Calculate a purchase's profit and worth live
        internal async Task<PurchaseModel> UpdatePurchaseAsync(PurchaseModel purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = await CryptoCompare.GetPriceAsync(crypto, purchase.Currency);

            var curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            /// If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty >= 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                purchase.Arrow = earningz < 0 ? "▼" : "▲";
                purchase.BoughtAt = priceBought;
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(Math.Abs(earningz), 2);
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
