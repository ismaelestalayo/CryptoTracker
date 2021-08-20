using CryptoTracker.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Shared.Constants;
using UWP.Shared.Helpers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class PortfolioList : UserControl {
        public PortfolioList() {
            InitializeComponent();
            DataContext = this;
        }

        public static readonly DependencyProperty PurchasesProperty =
        DependencyProperty.Register(
            nameof(Purchases),
            typeof(ObservableCollection<PurchaseModel>),
            typeof(PortfolioList),
            null);

        public ObservableCollection<PurchaseModel> Purchases {
            get => (ObservableCollection<PurchaseModel>)GetValue(PurchasesProperty);
            set => SetValue(PurchasesProperty, value);
        }

        public static readonly DependencyProperty ShowDetailsProperty =
        DependencyProperty.Register(
            nameof(ShowDetails),
            typeof(bool),
            typeof(PortfolioList),
            null);
        public bool ShowDetails {
            get => (bool)GetValue(ShowDetailsProperty);
            set => SetValue(ShowDetailsProperty, value);
        }

        public static readonly DependencyProperty GroupedProperty =
        DependencyProperty.Register(
            nameof(Grouped),
            typeof(bool),
            typeof(PortfolioList),
            new PropertyMetadata(false));
        public bool Grouped {
            get => (bool)GetValue(GroupedProperty);
            set => SetValue(GroupedProperty, value);
        }

        /// ##############################################################################
        /// Declare the delegate (if using non-generic pattern).
        /// public delegate void StringEventHandler(string val);

        public event EventHandler ClickGoTo;
        public event EventHandler UpdateParent;


        /// ##############################################################################
        private async void PurchaseDuplicate_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;
            var i = Purchases.IndexOf(purchase);
            var newPurchase = new PurchaseModel() {
                Crypto = purchase.Crypto,
                CryptoName = purchase.CryptoName,
                CryptoLogo = purchase.CryptoLogo,
                CryptoQty = purchase.CryptoQty,
                Currency = purchase.Currency,
                CurrencySymbol = purchase.CurrencySymbol,
                Id = Guid.NewGuid().ToString("N"),
                Type = purchase.Type,
                InvestedQty = purchase.InvestedQty,
                Date = purchase.Date,
                Exchange = purchase.Exchange,
                Notes = purchase.Notes
            };
            Purchases.Insert(i, newPurchase);

            var LocalPurchases = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio6);
            var match = LocalPurchases.Where(x => x.Id == purchase.Id).FirstOrDefault();
            var idx2 = LocalPurchases.IndexOf(match);
            if (idx2 >= 0) {
                LocalPurchases.Insert(idx2, newPurchase);
                await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, LocalPurchases);
            }
            else
                await new MessageDialog("Could not save changed to the LocalStorage",
                    "Error updating the Portfolio").ShowAsync();

            UpdateParent?.Invoke(null, null);
        }

        private async void PurchaseEdit_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;

            var dialog = new PortfolioEntryDialog() {
                NewPurchase = purchase,
                SuggestionCoin = new SuggestionCoin(purchase.Crypto, purchase.CryptoName),
                PrimaryButtonText = "Save",
                Title = "💵 Edit purchase",
                RequestedTheme = ColorConstants.CurrentThemeIsDark() ? ElementTheme.Dark : ElementTheme.Light
            };
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary) {
                /// the "purchase" object is binded TwoWay and updated by itself
                //if (App.CurrentPage == nameof(Views.Portfolio))
                var LocalPurchases = await PortfolioHelper.GetPortfolio();
                var match = LocalPurchases.Where(x => x.Id == dialog.NewPurchase.Id).FirstOrDefault();
                var idx2 = LocalPurchases.IndexOf(match);
                if (idx2 >= 0) {
                    LocalPurchases[idx2] = dialog.NewPurchase;
                    await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, LocalPurchases);
                }
                else
                    await new MessageDialog("Could not save changed to the LocalStorage",
                        "Error updating the Portfolio").ShowAsync();

                UpdateParent?.Invoke(null, null);
            }
        }


        private void PurchaseGoToCoin_Click(object sender, RoutedEventArgs e) {
            var item = ((MenuFlyoutItem)sender).DataContext as PurchaseModel;
            ClickGoTo?.Invoke(item.Crypto, null);
        }


        private async void PurchaseRemove_Click(object sender, RoutedEventArgs e) {
            var purchase = (PurchaseModel)((FrameworkElement)sender).DataContext;
            var LocalPurchases = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio6);

            if (Grouped) {
                var crypto = purchase.Crypto;
                Purchases.Remove(purchase);
                var matches = LocalPurchases.Where(x => x.Crypto == crypto).ToList();
                foreach (var match in matches)
                    LocalPurchases.Remove(match);
            }
            else {
                Purchases.Remove(purchase);
                LocalPurchases.Remove(LocalPurchases.Where(x => x.Id == purchase.Id).FirstOrDefault());
            }

            /// Save the portfolio and update the parent page
            await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, LocalPurchases);
            UpdateParent?.Invoke(null, null);
        }

    }
}
