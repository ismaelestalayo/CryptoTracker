using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {
	// ###############################################################################################
	/// <summary>
	/// News items for the AdaptiveGridView
	/// </summary>
	public class NewsItem {
        public int Type { get; set; }
        public string Message { get; set; }
        public List<object> Promoted { get; set; }
        public List<NewsData> Data { get; set; }
        public bool HasWarning { get; set; }
    }

    public class NewsData {
        public string id { get; set; }
        public string guid { get; set; }
        public int published_on { get; set; }
        public string imageurl { get; set; } = "ms-appx:///Assets/transparent.png";
        public string title { get; set; } = "Loading...";
        public string url { get; set; } = "null";
        public string source { get; set; } = "";
        public string body { get; set; }
        public string tags { get; set; }
        public string categories { get; set; } = "cat";
        public List<string> categorylist { get; set; } = new List<string>();
        public string upvotes { get; set; }
        public string downvotes { get; set; }
        public string lang { get; set; }
    }


    /// <summary>
    /// Categories to filter news for the TokenBox
    /// </summary>
    public class NewsCategories {
        public string categoryName { get; set; }
        public List<string> wordsAssociatedWithCategory { get; set; }
        public List<string> excludedPhrases { get; set; }
        public List<string> includedPhrases { get; set; }
    }



    // ###############################################################################################
    // ###############################################################################################
    // ###############################################################################################
    public sealed partial class News : Page {
        private NewsData[] _emptyNews = Enumerable.Repeat(new NewsData(), 30).ToArray();
        private List<String> _filters { get; set; }
        private List<NewsCategories> _categories { get; set; }
        private AdvancedCollectionView _acv;
        private TokenizingTextBox _ttb;

        public News() {
            this.InitializeComponent();

            _categories = GetNewsCategories().Result;
            _filters = new List<string>();

            GetNews();
            _acv = new AdvancedCollectionView(_categories, false);
            _acv.SortDescriptions.Add(new SortDescription(nameof(NewsCategories.categoryName), SortDirection.Ascending));

            Loaded += (sender, e) => { this.OnXamlRendered(this); };
        }
        

        public void OnXamlRendered(FrameworkElement control) {

            if (control.FindChildByName("CategoriesTokenBox") is TokenizingTextBox ttb) {
                _ttb = ttb;
                _ttb.TokenItemAdded += TokenItemAdded;
                _ttb.TokenItemRemoving += TokenItemRemoved;
                _ttb.TextChanged += TextChanged;
                _ttb.TokenItemAdding += TokenItemCreating;
                _ttb.Tapped += TokenBox_Tapped;

                _acv.Filter = item => !_ttb.Items.Contains(item) && (item as NewsCategories).categoryName.Contains(_ttb.Text, StringComparison.CurrentCultureIgnoreCase);

                _ttb.SuggestedItemsSource = _acv;
            }
        }

        // ###############################################################################################
        internal async Task GetNews() {
            NewsAdaptiveGridView.IsItemClickEnabled = false;
            NewsAdaptiveGridView.ItemsSource = _emptyNews;

            string URL = "https://min-api.cryptocompare.com/data/v2/news/?lang=EN";
            if (_filters.Count > 0)
                URL += string.Format("&categories={0}", string.Join(",", _filters));
            
            Uri uri = new Uri(URL);
            HttpResponseMessage httpResponse = new HttpResponseMessage();


            try {
                httpResponse = await App.Client.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                var response = await httpResponse.Content.ReadAsStringAsync();
                var news = JsonSerializer.Deserialize<NewsItem>(response);
                foreach (NewsData n in news.Data) {
                    n.categorylist = n.categories.Split('|').ToList();
                    if (n.categorylist.Count > 3)
                        n.categorylist = n.categorylist.GetRange(1, 3);
                }
                NewsAdaptiveGridView.ItemsSource = news.Data;
                NewsAdaptiveGridView.IsItemClickEnabled = true;

            } catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        private async Task<List<NewsCategories>> GetNewsCategories() {
            string URL = "https://min-api.cryptocompare.com/data/news/categories";

            Uri uri = new Uri(URL);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            List<NewsCategories> cat;
            try {
                httpResponse = await App.Client.GetAsync(uri).ConfigureAwait(false);
                httpResponse.EnsureSuccessStatusCode();

                var response = await httpResponse.Content.ReadAsStringAsync();

                cat = JsonSerializer.Deserialize<List<NewsCategories>>(response);
            }
            catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
                cat = new List<NewsCategories>() { new NewsCategories() };
            }
            return cat;
        }

        // ###############################################################################################
        //  AdaptiveGridView Elements
        private void NewsAdaptiveGridView_Click(object sender, ItemClickEventArgs e) {
            NewsAdaptiveGridView.PrepareConnectedAnimation("toWebView", e.ClickedItem, "GridView_Element");
            this.Frame.Navigate(typeof(WebVieww), ((NewsData)e.ClickedItem).url);
        }

        // ###############################################################################################
        //  TokenizingTextBox
        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            if (args.CheckCurrent() && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                _acv.RefreshFilter();
        }
        private void TokenItemAdded(TokenizingTextBox sender, object data) {
            if (data is NewsCategories category && sender.Items.Count < 5) {
                _filters.Add(category.categoryName);
                GetNews();
            }
        }

        private void TokenItemRemoved(TokenizingTextBox sender, TokenItemRemovingEventArgs args) {
            if (args.Item is NewsCategories category) {
                _filters.Remove(category.categoryName);
                GetNews();
            }
        }

        private void TokenItemCreating(object sender, TokenItemAddingEventArgs e) {
            // Take the user's text and convert it to our data type (if we have a matching one).
            e.Item = _categories.FirstOrDefault((item) => item.categoryName.Contains(e.TokenText, StringComparison.CurrentCultureIgnoreCase));
            if (e.Item == null)
                e.Cancel = true;
        }

        private void TokenBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            _ttb.Focus(FocusState.Programmatic); // Give focus back to type another filter
        }
    }
}
