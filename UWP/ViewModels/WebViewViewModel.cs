using Microsoft.Toolkit.Mvvm.ComponentModel;
using UWP.Models;

namespace UWP.ViewModels {
    partial class WebViewViewModel : ObservableObject {

        [ObservableProperty]
        private NewsData news = new NewsData();
    }
}
