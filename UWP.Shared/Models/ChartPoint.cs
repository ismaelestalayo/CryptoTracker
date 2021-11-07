using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    // #########################################################################################
	//  Object for plotting charts
	public class ChartPoint {
        public DateTime Date { get; set; }
        public double Value { get; set; } = 0;
        public double Low { get; set; } = 0;
        public double High { get; set; } = 0;
        public double Open { get; set; } = 0;
        public double Close { get; set; } = 0;
        public double Volume { get; set; } = 0;
        public string Category { get; set; }
        public Color Color { get; set; } = Color.FromArgb(255, 128, 128, 128);
        public SolidColorBrush Brush { get; set; } = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128));
    }
}
