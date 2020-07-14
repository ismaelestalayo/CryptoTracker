using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace CryptoTracker.Views {
    // ###############################################################################################
    public class SourceInfo {
        public string name { get; set; }
        public string lang { get; set; }
        public string img { get; set; }
    }

    public class NewsData {
        public string id { get; set; }
        public string guid { get; set; }
        public int published_on { get; set; }
        public string imageurl { get; set; }
        public string title { get; set; } = "null";
        public string url { get; set; } = "null";
        public string source { get; set; } = "null";
        public string body { get; set; }
        public string tags { get; set; }
        public string categories { get; set; } = "null";
        public List<string> categorylist { get; set; }
        public string upvotes { get; set; }
        public string downvotes { get; set; }
        public string lang { get; set; }
        public SourceInfo source_info { get; set; }
    }

    // News item for the AdaptiveGridView
    public class NewsItem {
        public int Type { get; set; }
        public string Message { get; set; }
        public List<object> Promoted { get; set; }
        public List<NewsData> Data { get; set; }
        public bool HasWarning { get; set; }
    }



    // ###############################################################################################
    // ###############################################################################################
    // ###############################################################################################
    public sealed partial class News : Page {

        public News() {
            this.InitializeComponent();

            getNewsAsync();
        }

        

        // ###############################################################################################
        private async Task getNewsAsync() {
            string URL = "https://min-api.cryptocompare.com/data/v2/news/?lang=EN";

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            try {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                var response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                var news = JToken.Parse(data.ToString()).ToObject<NewsItem>();
                foreach (NewsData n in news.Data) {
                    n.categorylist = n.categories.Split('|').ToList();
                    if (n.categorylist.Count > 3)
                        n.categorylist = n.categorylist.GetRange(1, 3);
                }
                NewsAdaptiveGridView.ItemsSource = news.Data;

            } catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
            }
            finally {
                httpClient.Dispose();
                httpResponse.Dispose();
            }
        }

        // ###############################################################################################
        //  AdaptiveGridView Elements
        private void NewsAdaptiveGridView_Click(object sender, ItemClickEventArgs e) {

            NewsAdaptiveGridView.PrepareConnectedAnimation("toWebView", e.ClickedItem, "GridView_Element");
            this.Frame.Navigate(typeof(WebVieww), ((NewsData)e.ClickedItem).url);
        }
    }
}
