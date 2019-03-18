using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {

    public class NewsItem {
        public string imageURL  { get; set; }
        public string title     { get; set; }
        public string url       { get; set; }
        public string source    { get; set; }
        public string body      { get; set; }
        public string tags      { get; set; }
        public string language  { get; set; }
    }

    // ###############################################################################################
    // ###############################################################################################
    // ###############################################################################################
    public sealed partial class News : Page {

        private List<NewsItem> NewsTilesList { get; set; }

        Compositor _compositor = Window.Current.Compositor;


        public News() {
            this.InitializeComponent();

            NewsTilesList = new List<NewsItem>();
            getNewsAsync();
        }

        

        // ###############################################################################################
        private async Task getNewsAsync() {
            String URL = "https://min-api.cryptocompare.com/data/v2/news/?lang=EN";

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            String response = "";

            try {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                int lastIndex = ((JContainer)data["Data"]).Count;
                for (int i = 0; i < lastIndex; i++) {
                    NewsTilesList.Add(new NewsItem {
                        imageURL    = data["Data"][i]["imageurl"].ToString(),
                        title       = data["Data"][i]["title"].ToString(),
                        url         = data["Data"][i]["url"].ToString(),
                        source      = data["Data"][i]["source"].ToString(),
                        body        = data["Data"][i]["body"].ToString(),
                        tags        = data["Data"][i]["tags"].ToString(),
                        language    = data["Data"][i]["lang"].ToString(),
                    });
                    newsAdaptiveGridView.ItemsSource = NewsTilesList;
                }
                
                //newsAdaptiveGridView.ItemsSource = NewsTilesList;

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        // ###############################################################################################
        private void newsAdaptiveGridView_Click(object sender, ItemClickEventArgs e) {

            newsAdaptiveGridView.PrepareConnectedAnimation("toWebView", e.ClickedItem, "GridView_Element");

            this.Frame.Navigate(typeof(WebVieww), ((NewsItem)e.ClickedItem).url);
        }

        

    }
}
