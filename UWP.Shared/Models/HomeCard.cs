using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace UWP.Models {
	public partial class HomeCard : ObservableObject {
		[ObservableProperty]
		private Coin info = new Coin();

		[ObservableProperty]
		private ChartModel chart = new ChartModel();
	}
}
