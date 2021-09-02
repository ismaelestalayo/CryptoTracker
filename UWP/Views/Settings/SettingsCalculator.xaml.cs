using System.Collections.Generic;
using System.Collections.ObjectModel;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsCalculator : Page {

        private List<double> Prices { get; set; }
        private ObservableCollection<SuggestionCoin> Coins { get; set; }

        public SettingsCalculator() {
            InitializeComponent();
            Loaded += SettingsCalculator_Loaded;
        }

        private void SettingsCalculator_Loaded(object sender, RoutedEventArgs e) {
            Coins = new ObservableCollection<SuggestionCoin>() {
                new SuggestionCoin(), new SuggestionCoin()
            };
        }


        // ###############################################################################################


    }
}
