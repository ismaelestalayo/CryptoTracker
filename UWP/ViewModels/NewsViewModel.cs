using CryptoTracker.Models;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace CryptoTracker.ViewModels {
    class NewsViewModel : ObservableRecipient {

        private List<NewsData> news = Enumerable.Repeat(new NewsData(), 30).ToList();
        public List<NewsData> News {
            get => news;
            set => SetProperty(ref news, value);
        }

        public List<NewsCategories> Categories { get; set; }
        public List<string> Filters { get; set; } = new List<string>();
    }
}
