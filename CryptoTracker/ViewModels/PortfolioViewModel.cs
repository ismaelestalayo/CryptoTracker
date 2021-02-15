using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.ViewModels {
	public class PortfolioViewModel : ObservableObject {

		public Grid DiversificationGrid;

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
	}
}
