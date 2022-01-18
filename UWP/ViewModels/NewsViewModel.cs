using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using UWP.Models;

namespace UWP.ViewModels {
    partial class NewsViewModel : ObservableRecipient {

        [ObservableProperty]
        private List<NewsData> news = Enumerable.Repeat(new NewsData(), 30).ToList();


        public List<NewsCategories> Categories { get; set; }
        public List<string> Filters { get; set; } = new List<string>();
    }
}
