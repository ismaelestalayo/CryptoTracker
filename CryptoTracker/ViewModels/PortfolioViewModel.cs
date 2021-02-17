using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace CryptoTracker.ViewModels {
    public class PortfolioViewModel : ObservableObject {

		private ObservableCollection<PurchaseModel> purchaseList = new ObservableCollection<PurchaseModel>();
		public ObservableCollection<PurchaseModel> PurchaseList {
			get => purchaseList;
			set => SetProperty(ref purchaseList, value);
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

		private bool notEmptyPortfolio = true;
		public bool NotEmptyPortfolio {
			get => notEmptyPortfolio;
			set => SetProperty(ref notEmptyPortfolio, value);
		}
	}
}
