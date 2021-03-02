using System.Collections.Generic;

namespace CryptoTracker.Models {
    /// <summary>
    /// News items for the AdaptiveGridView
    /// </summary>
    public class NewsResponse {
        public int Type { get; set; }
        public string Message { get; set; }
        public List<object> Promoted { get; set; }
        public List<NewsData> Data { get; set; }
        public bool HasWarning { get; set; }
    }

    public class NewsData {
        public string id { get; set; }
        public string guid { get; set; }
        public int published_on { get; set; }
        public string imageurl { get; set; } = "ms-appx:///Assets/transparent.png";
        public string title { get; set; } = "Loading...";
        public string url { get; set; } = "null";
        public string source { get; set; } = "";
        public string body { get; set; }
        public string tags { get; set; }
        public string categories { get; set; } = "cat";
        public List<string> categorylist { get; set; } = new List<string>();
        public string upvotes { get; set; }
        public string downvotes { get; set; }
        public string lang { get; set; }
    }

    /// <summary>
    /// Categories to filter news for the TokenBox
    /// </summary>
    public class NewsCategories {
        public string categoryName { get; set; }
        public List<string> wordsAssociatedWithCategory { get; set; }
        public List<string> excludedPhrases { get; set; }
        public List<string> includedPhrases { get; set; }
    }
}
