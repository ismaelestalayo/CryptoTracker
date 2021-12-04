using Microsoft.Toolkit.Mvvm.ComponentModel;
using UWP.Models;

namespace UWP.ViewModels {
    partial class CoinCompactViewModel : ObservableObject {

		[ObservableProperty]
		private Coin info = new Coin();

		
		[ObservableProperty]
		private ChartModel chart = new ChartModel();


		[ObservableProperty]
		private CoinDetailsViewModel coinDetailsVM;
	}
}
