using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace UWP.Models {
    public class SuggestionCoin : ObservableObject {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
