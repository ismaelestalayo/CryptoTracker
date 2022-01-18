using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.APIs;
using UWP.Models;
using UWP.Shared.Interfaces;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {

    public sealed partial class News : Page, UpdatablePage {
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

            await UpdatePage();
        }

        public async Task UpdatePage() {
            NewsAdaptiveGridView.IsItemClickEnabled = false;
            var news = await CryptoCompare.GetNews(vm.Filters);
            if (news.Count != 0) {
                // Filter out websites that crash the WebView:
                news = news.FindAll(x => !x.url.Contains("ambcrypto"));

                vm.News = news;
                NewsAdaptiveGridView.IsItemClickEnabled = true;
            } else
                vm.News = new List<NewsData>() { new NewsData() { title = "Error getting the news..." } };
        }

        /// ###############################################################################################
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
                UpdatePage();
            }
        }

        private void TokenItemRemoved(TokenizingTextBox sender, TokenItemRemovingEventArgs args) {
            if (args.Item is NewsCategories category) {
                vm.Filters.Remove(category.categoryName);
                UpdatePage();
            }
        }

        private void TokenBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            _ttb.Focus(FocusState.Programmatic); // Give focus back to type another filter
        }
    }
}
