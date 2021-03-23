using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    // #########################################################################################
	//  Object for plotting charts
	public class ChartPoint {
        public DateTime Date { get; set; }
        public double Value { get; set; }
        public double Low { get; set; }
        public double High { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double Volume { get; set; }
        public string Category { get; set; }
        public Color Color { get; set; } = Color.FromArgb(255, 128, 128, 128);
        public SolidColorBrush Brush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
    }
}
