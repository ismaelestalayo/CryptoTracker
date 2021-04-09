using Microsoft.Toolkit.Mvvm.ComponentModel;
using UWP.Models;

namespace UWP.ViewModels {
    class CoinCompactViewModel : ObservableObject {

		private Coin info = new Coin();
		public Coin Info {
			get => info;
			set => SetProperty(ref info, value);
		}

		private ChartModel chart = new ChartModel();
		public ChartModel Chart {
			get => chart;
			set => SetProperty(ref chart, value);
		}

		private CoinDetailsViewModel coinDetailsVM;
		public CoinDetailsViewModel CoinDetailsVM {
			get => coinDetailsVM;
			set => SetProperty(ref coinDetailsVM, value);
		}
	}
}
