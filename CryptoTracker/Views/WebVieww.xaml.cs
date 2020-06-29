using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker.Views {
    public sealed partial class WebVieww : Page {
        public WebVieww() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

            if (e != null) { 
                // Connected animation
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toWebView");
                if (animation != null) {
                    animation.TryStart(myWebView);
                }

                String url = e.Parameter.ToString();
                if (url.StartsWith("http"))
                    myWebView.Navigate(new Uri(url));
                else if (url.StartsWith("@"))
                    myWebView.Navigate(new Uri(string.Format("https://twitter.com/{0}", url)));
                else { }

            }
        }

        private void MyWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {
            urlBar.Text = args.Uri.ToString();
        }

        private void MyWebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {
            urlBar.Text = args.Uri.ToString();
        }
    }
}
