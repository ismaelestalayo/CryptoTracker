using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

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
        //  AdaptiveGridView Elements
        private void newsAdaptiveGridView_Click(object sender, ItemClickEventArgs e) {

            newsAdaptiveGridView.PrepareConnectedAnimation("toWebView", e.ClickedItem, "GridView_Element");
            this.Frame.Navigate(typeof(WebVieww), ((NewsItem)e.ClickedItem).url);
        }

        private void newsAdaptiveGridView_ContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args) {
            var elementVisual = ElementCompositionPreview.GetElementVisual(args.ItemContainer);
            if (args.InRecycleQueue) {
                elementVisual.ImplicitAnimations = null;
            } 
            else {
                if (elementVisual.ImplicitAnimations == null) {
                    var compositor =
                        ElementCompositionPreview.GetElementVisual(this).Compositor;

                    var offsetAnimation = compositor.CreateVector3KeyFrameAnimation();
                    offsetAnimation.Target = nameof(Visual.Offset);
                    offsetAnimation.InsertExpressionKeyFrame(1.0f, "this.FinalValue");
                    offsetAnimation.Duration = TimeSpan.FromMilliseconds(400);

                    var rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
                    rotationAnimation.Target = nameof(Visual.RotationAngle);
                    rotationAnimation.InsertKeyFrame(.5f, 0.160f);
                    rotationAnimation.InsertKeyFrame(1f, 0f);
                    rotationAnimation.Duration = TimeSpan.FromSeconds(400);

                    var animationGroup = compositor.CreateAnimationGroup();
                    animationGroup.Add(offsetAnimation);
                    animationGroup.Add(rotationAnimation);

                    elementVisual.ImplicitAnimations = compositor.CreateImplicitAnimationCollection();
                    elementVisual.ImplicitAnimations[nameof(Visual.Offset)] = animationGroup;
                }
                elementVisual.ImplicitAnimations = elementVisual.ImplicitAnimations;
            }
        }
    }
}
