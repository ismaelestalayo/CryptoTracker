using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP.Models;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsCalculator : Page {

        private List<double> Prices { get; set; }
        private ObservableCollection<SuggestionCoin> Coins { get; set; }

        public SettingsCalculator() {
            this.InitializeComponent();
            Coins = new ObservableCollection<SuggestionCoin>() {
                new SuggestionCoin(), new SuggestionCoin()
            };
        }


        // ###############################################################################################

        
    }
}
