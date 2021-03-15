using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;

namespace CryptoTracker.ViewModels {
    class WebViewViewModel : ObservableObject {
        
        private NewsData news = new NewsData();
        public NewsData News {
            get => news;
            set => SetProperty(ref news, value);
        }
    }
}
