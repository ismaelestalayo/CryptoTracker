using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace CryptoTracker.ViewModels {
	public class Top100ViewModel : ObservableObject {

		private GlobalStats _globalStats;
		public GlobalStats GlobalStats {
			get => _globalStats;
			set => SetProperty(ref _globalStats, value);
		}

		private List<Top100card> _top100cards;
		public List<Top100card> Top100cards {
			get => _top100cards;
			set => SetProperty(ref _top100cards, value);
		}
	}
}
