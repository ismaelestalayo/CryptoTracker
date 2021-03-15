using UWP.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace UWP.ViewModels {
	public class HomeViewModel : ObservableObject {

		private ObservableCollection<HomeCard> priceCards = new ObservableCollection<HomeCard>();
		public ObservableCollection<HomeCard> PriceCards {
			get => priceCards;
			set => SetProperty(ref priceCards, value);
		}

		private ObservableCollection<HomeCard> volumeCards = new ObservableCollection<HomeCard>();
		public ObservableCollection<HomeCard> VolumeCards {
			get => volumeCards;
			set => SetProperty(ref volumeCards, value);
		}
	}
}
