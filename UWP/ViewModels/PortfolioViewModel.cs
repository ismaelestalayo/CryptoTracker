using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;

namespace UWP.ViewModels {
    public class PortfolioViewModel : ObservableRecipient {

		public PortfolioViewModel() {  }

        private ObservableCollection<PurchaseModel> portfolio = new ObservableCollection<PurchaseModel>();
		public ObservableCollection<PurchaseModel> Portfolio {
			get => portfolio;
			set => SetProperty(ref portfolio, value);
		}

		private PurchaseModel newPurchase = new PurchaseModel();
		public PurchaseModel NewPurchase {
			get => newPurchase;
			set => SetProperty(ref newPurchase, value);
		}

		private ChartModel chart = new ChartModel();
		public ChartModel Chart {
			get => chart;
			set => SetProperty(ref chart, value);
		}

		/// <summary>
		/// Single variables for the page
		/// </summary>
		private string currencySym = App.currencySymbol;
		public string CurrencySymbol {
			get => currencySym;
			set => SetProperty(ref currencySym, value);
		}

		private bool _purchasesAreGrouped = false;
		public bool PurchasesAreGrouped {
			get => _purchasesAreGrouped;
			set => SetProperty(ref _purchasesAreGrouped, value);
		}

		private bool _showDetails = false;
		public bool ShowDetails {
			get => _showDetails;
			set => SetProperty(ref _showDetails, value);
		}

		/// #############################################################################
		/// Total values
		private double totalInvested = 0;
		public double TotalInvested {
			get => totalInvested;
			set => SetProperty(ref totalInvested, value);
		}
		private double totalWorth = 0;
		public double TotalWorth {
			get => totalWorth;
			set => SetProperty(ref totalWorth, value);
		}
		private double totalDelta = 0;
		public double TotalDelta {
			get => totalDelta;
			set => SetProperty(ref totalDelta, value);
		}


		private string allPurchasesCurrencySym = "";
		public string AllPurchasesCurrencySym {
			get => allPurchasesCurrencySym;
			set => SetProperty(ref allPurchasesCurrencySym, value);
		}
		private bool allPurchasesInCurrency = true;
		public bool AllPurchasesInCurrency {
			get => allPurchasesInCurrency;
			set => SetProperty(ref allPurchasesInCurrency, value);
		}

		/// #############################################################################
		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
