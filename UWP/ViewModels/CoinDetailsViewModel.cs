using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UWP.Models;
using UWP.Shared.Helpers;
using static UWP.APIs.CoinGecko;

namespace UWP.ViewModels {
    partial class CoinDetailsViewModel : ObservableRecipient {

        [ObservableProperty]
        private CoinData coinInfo = new CoinData();

        [ObservableProperty]
        private Coin coin = new Coin();

        [ObservableProperty]
        private ChartModel chart = new ChartModel();

        [ObservableProperty]
        private ChartPoint chartPoint = new ChartPoint();

        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(TotalCryptoQty))]
        [AlsoNotifyChangeFor(nameof(TotalInvested))]
        [AlsoNotifyChangeFor(nameof(TotalProfit))]
        [AlsoNotifyChangeFor(nameof(TotalValue))]
        [AlsoNotifyChangeFor(nameof(TotalAvgPrice))]
        private ObservableCollection<PurchaseModel> purchases;

        [ObservableProperty]
        private ObservableCollection<Alert> alerts;

        [ObservableProperty]
        private bool showCandles = false;

        [ObservableProperty]
        private bool showDetails = false;

        /// <summary>
        /// Last update date to also trigger update on total values
        /// </summary>
        [ObservableProperty]
        [AlsoNotifyChangeFor(nameof(TotalCryptoQty))]
        [AlsoNotifyChangeFor(nameof(TotalInvested))]
        [AlsoNotifyChangeFor(nameof(TotalProfit))]
        [AlsoNotifyChangeFor(nameof(TotalValue))]
        [AlsoNotifyChangeFor(nameof(TotalAvgPrice))]
        private DateTime lastUpdate;


        /// <summary>
        /// Single variables for the page
        /// </summary>
        [ObservableProperty]
        private string currencySymbol = App.currencySymbol;


        /// <summary>
        /// Total sum of purchases
        /// </summary>
        internal double TotalAvgPrice {
            get => NumberHelper.Rounder(TotalInvested / TotalCryptoQty);
        }

        internal double TotalCryptoQty {
            get => purchases?.Select(x => x.CryptoQty).Sum() ?? 0;
        }

        internal double TotalValue {
            get => TotalCryptoQty * coin.Price;
        }

        public double TotalInvested {
            get => purchases?.Select(x => x.InvestedQty).Sum() ?? 0;
        }

        internal double TotalProfit {
            get => purchases?.Select(x => x.Profit).Sum() ?? 0;
        }


        /// #############################################################################
        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
