using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.ViewModels {
    public class PortfolioViewModel : ObservableRecipient {

		public PortfolioViewModel() {
			Messenger.Register<PortfolioViewModel, PortfolioMessage>(this, (r, m) => {
				Portfolio = m.Value;
			});
		}

		//protected override void OnActivated() {
		//	Messenger.Register<PortfolioViewModel, PortfolioMessage>(this, (r, m) => {
		//		PurchaseList = m.Value;
		//	});
		//}

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

		private ObservableCollection<string> coinsArray = new ObservableCollection<string>();
		public ObservableCollection<string> CoinsArray {
			get => coinsArray;
			set => SetProperty(ref coinsArray, value);
		}


		private ChartModel chart = new ChartModel();
		public ChartModel Chart {
			get => chart;
			set => SetProperty(ref chart, value);
		}

		/// <summary>
		/// Single variables for the page
		/// </summary>
		private string currency = App.currencySymbol;
		public string Currency {
			get => currency;
			set => SetProperty(ref currency, value);
		}

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

		// To toggle the visibility of certain elements
		private bool populatedPortfolio = false;
		public bool PopulatedPortfolio {
			get => populatedPortfolio;
			set => SetProperty(ref populatedPortfolio, value);
		}
	}
}
