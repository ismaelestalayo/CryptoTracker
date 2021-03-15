using UWP.Models;
using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace UWP.Views {
    public sealed partial class WebVieww : Page {
        public WebVieww() {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;

            if (e != null && e.Parameter is NewsData news) { 
                // Connected animation
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toWebView");
                if (animation != null)
                    animation.TryStart(myWebView);

                vm.News = news;
                var url = vm.News.url;
                if (url.StartsWith("http"))
                    myWebView.Navigate(new Uri(url));
            }
        }

        private void MyWebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {
            vm.News.url = args.Uri.ToString();
        }

        private void GoBack_click(object sender, RoutedEventArgs e) {
            Frame.GoBack();
        }

        private async void OpenInBrowser_click(object sender, RoutedEventArgs e) {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(vm.News.url));
        }

        private void Share_click(object sender, RoutedEventArgs e) {
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args) {
            DataRequest request = args.Request;
            request.Data.Properties.Title = vm.News.title;
            request.Data.SetWebLink(new Uri(vm.News.url));
        }
    }
}
