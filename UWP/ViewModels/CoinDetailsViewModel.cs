using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;
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
        private ObservableCollection<PurchaseModel> purchases;

        [ObservableProperty]
        private ObservableCollection<Alert> alerts;

        [ObservableProperty]
        private bool showCandles = false;


        /// <summary>
        /// Single variables for the page
        /// </summary>
        [ObservableProperty]
        private string currencySymbol = App.currencySymbol;


        /// <summary>
        /// Total sum of purchases
        /// </summary>
        [ObservableProperty]
        private double avgPrice = 0;

        [ObservableProperty]
        private double totalQty = 0;

        [ObservableProperty]
        private double totalValue = 0;

        [ObservableProperty]
        private double totalProfit = 0;


        /// #############################################################################
        public void InAppNotification(string title, string message = "") {
            var tuple = new Tuple<string, string>(title, message);
            Messenger.Send(new NotificationMessage(tuple));
        }
    }
}
