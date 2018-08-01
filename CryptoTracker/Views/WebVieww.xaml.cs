using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker.Views {
    public sealed partial class WebVieww : Page {
        public WebVieww() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

            String crypto = e.Parameter.ToString();
            switch (crypto) {
                case "BTC_Website":
                    myWebView.Navigate(new Uri("https://bitcoin.org/"));
                    break;
                case "BTC_Twitter":
                    myWebView.Navigate(new Uri("https://mobile.twitter.com/search?q=bitcoin"));
                    break;
                case "BTC_Reddit":
                    myWebView.Navigate(new Uri("https://www.reddit.com/r/bitcoin"));
                    break;

                case "ETH_Website":
                    myWebView.Navigate(new Uri("https://www.ethereum.org/"));
                    break;
                case "ETH_Twitter":
                    myWebView.Navigate(new Uri("https://mobile.twitter.com/ethereumproject"));
                    break;
                case "ETH_Reddit":
                    myWebView.Navigate(new Uri("https://www.reddit.com/r/ethereum"));
                    break;

                case "LTC_Website":
                    myWebView.Navigate(new Uri("https://litecoin.org/"));
                    break;
                case "LTC_Twitter":
                    myWebView.Navigate(new Uri("https://mobile.twitter.com/litecoinproject"));
                    break;
                case "LTC_Reddit":
                    myWebView.Navigate(new Uri("https://www.reddit.com/r/litecoin"));
                    break;

                case "XRP_Website":
                    myWebView.Navigate(new Uri("https://ripple.com/"));
                    break;
                case "XRP_Twitter":
                    myWebView.Navigate(new Uri("https://mobile.twitter.com/ripple"));
                    break;
                case "XRP_Reddit":
                    myWebView.Navigate(new Uri("https://www.reddit.com/r/ripple"));
                    break;

                default:
                    myWebView.Navigate(new Uri(crypto));
                    break;
            }

        }

        private void myWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {
            urlBar.Text = args.Uri.ToString();
        }

        private void myWebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args) {
            urlBar.Text = args.Uri.ToString();
        }
    }
}
