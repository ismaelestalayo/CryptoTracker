using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.ObjectModel;
using UWP.Models;

namespace UWP.ViewModels {
    public partial class PortfolioViewModel : ObservableRecipient {

		public PortfolioViewModel() {  }

		[ObservableProperty]
        private ObservableCollection<PurchaseModel> portfolio = new ObservableCollection<PurchaseModel>();

		[ObservableProperty]
		private PurchaseModel newPurchase = new PurchaseModel();

		[ObservableProperty]
		private ChartModel chart = new ChartModel();


		/// <summary>
		/// Single variables for the page
		/// </summary>
		[ObservableProperty]
		private string currencySymbol = App.currencySymbol;

		[ObservableProperty]
		private bool purchasesAreGroupable = false;

		[ObservableProperty]
		private bool purchasesAreGrouped = false;

		[ObservableProperty]
		private bool showDetails = false;


		/// #############################################################################
		/// Total values
		[ObservableProperty]
		private double totalInvested = 0;

		[ObservableProperty]
		private double totalWorth = 0;
		
		[ObservableProperty]
		private double totalDelta = 0;
		
		private double roi = 0;
		public double ROI {
			get => roi;
			set => SetProperty(ref roi, value);
		}


		[ObservableProperty]
		private string allPurchasesCurrencySym = "";

		[ObservableProperty]
		private bool allPurchasesInCurrency = true;


		/// #############################################################################
		public void InAppNotification(string title, string message = "") {
			var tuple = new Tuple<string, string>(title, message);
			Messenger.Send(new NotificationMessage(tuple));
		}
	}
}
