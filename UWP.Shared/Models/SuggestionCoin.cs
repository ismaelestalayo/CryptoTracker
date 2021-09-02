using Microsoft.Toolkit.Mvvm.ComponentModel;
using UWP.Helpers;

namespace UWP.Models {
    public class SuggestionCoin : ObservableObject {
        public SuggestionCoin() { }
        public SuggestionCoin(Coin coin) {
            Icon = IconsHelper.GetIcon(coin.Name);
            Name = coin.FullName;
            Symbol = coin.Name;
        }
        public SuggestionCoin(string name, string fullName) {
            Icon = IconsHelper.GetIcon(name);
            Name = fullName;
            Symbol = name;
        }

        public string Icon { get; set; } = "/Assets/Icons/iconNULL.png";
        public string Name { get; set; } = "Null";
        public string Symbol { get; set; } = "NULL";
    }
}
