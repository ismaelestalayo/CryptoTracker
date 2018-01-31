using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;


namespace CryptoTracker.Views {
    public sealed partial class EditWatchlist : Page {

        private ObservableCollection<string> suggestions;

        public EditWatchlist() {
            this.InitializeComponent();

            suggestions = new ObservableCollection<string>();
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                suggestions.Clear();
                sender.ItemsSource = App.coinList.Where(x => x.StartsWith(sender.Text)).ToList();
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {
            CoinAutoSuggestBox.Text = args.SelectedItem.ToString();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            if (args.ChosenSuggestion != null)
                this.Frame.Navigate(typeof(CoinDetails), CoinAutoSuggestBox.Text);

            else
                CoinAutoSuggestBox.Text = sender.Text;
        }
    }
}
