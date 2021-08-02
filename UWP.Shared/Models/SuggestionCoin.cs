using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace UWP.Models {
    public class SuggestionCoin : ObservableObject {
        public string Icon { get; set; } = "/Assets/Icons/iconNULL.png";
        public string Name { get; set; } = "Null";
        public string Symbol { get; set; } = "NULL";
    }
}
