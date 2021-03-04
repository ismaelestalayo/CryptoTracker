using CryptoTracker.APIs;
using CryptoTracker.Models;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {

    public sealed partial class News : Page {
        /// Private variables
        private AdvancedCollectionView _acv;
        private TokenizingTextBox _ttb;


        public News() {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e) {
            vm.Categories = await CryptoCompare.GetNewsCategories();
            _acv = new AdvancedCollectionView(vm.Categories, false);
            _acv.SortDescriptions.Add(new SortDescription(nameof(NewsCategories.categoryName), SortDirection.Ascending));

            _ttb = CategoriesTokenBox;
            _acv.Filter = item => !_ttb.Items.Contains(item) && (item as NewsCategories).categoryName.Contains(_ttb.Text, StringComparison.CurrentCultureIgnoreCase);
            _ttb.SuggestedItemsSource = _acv;

            await UpdateNews();
        }

        /// ###############################################################################################
        internal async Task UpdateNews() {
            NewsAdaptiveGridView.IsItemClickEnabled = false;
            vm.News = await CryptoCompare.GetNews(vm.Filters);
            NewsAdaptiveGridView.IsItemClickEnabled = true;
        }

        private void NewsItem_Click(object sender, ItemClickEventArgs e) {
            NewsAdaptiveGridView.PrepareConnectedAnimation("toWebView", e.ClickedItem, "GridView_Element");
            this.Frame.Navigate(typeof(WebVieww), ((NewsData)e.ClickedItem));
        }

        /// ###############################################################################################
        ///  TokenizingTextBox
        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (args.CheckCurrent() && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                _acv.RefreshFilter();
        }
        private void TokenItemAdded(TokenizingTextBox sender, object data) {
            if (data is NewsCategories category && sender.Items.Count < 5) {
                vm.Filters.Add(category.categoryName);
                UpdateNews();
            }
        }

        private void TokenItemRemoved(TokenizingTextBox sender, TokenItemRemovingEventArgs args) {
            if (args.Item is NewsCategories category) {
                vm.Filters.Remove(category.categoryName);
                UpdateNews();
            }
        }

        private void TokenBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            _ttb.Focus(FocusState.Programmatic); // Give focus back to type another filter
        }
    }
}
