using System;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Helpers {
    // #########################################################################################
    //  Object for plotting charts
    public class ChartData {
        public DateTime Date    { get; set; }
        public float Value      { get; set; }
        public float Low        { get; set; }
        public float High       { get; set; }
        public float Open       { get; set; }
        public float Close      { get; set; }
        public float Volume     { get; set; }
        public string Category  { get; set; }
        public Brush cc         { get; set; }
    }
}
