using Microsoft.Toolkit.Mvvm.ComponentModel;
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
		private string currencySym = App.currencySymbol;
		public string CurrencySymbol {
			get => currencySym;
			set => SetProperty(ref currencySym, value);
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
	}
}
