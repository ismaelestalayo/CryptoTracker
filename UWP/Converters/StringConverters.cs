using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UWP.Shared.Constants;
using Windows.UI.Xaml.Data;

namespace UWP.Converters {
    public class StringFormatConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            try {
                var str = val.ToString();
                var num = double.Parse(str);
                if (!string.IsNullOrEmpty(str))
                    return num.ToString("N0", CultureInfo.CurrentCulture.NumberFormat);
                return val;
            }
            catch {
                return val;
            }
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class UpperCaseConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => val.ToString().ToUpperInvariant();

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class FavIconConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (bool)val ? "\uEB52" : "\uEB51";

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class StringNullOrEmptyToBoolConverter : IValueConverter {
        /// <summary>
        /// Determines whether an inverse conversion should take place.
        /// </summary>
        /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
        public bool Inverse { get; set; }

        public object Convert(object val, Type targetType, object param, string lang)
            => Inverse ? !string.IsNullOrWhiteSpace((string)val) : string.IsNullOrWhiteSpace((string)val);

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => string.Empty;
    }

    public class MicroChartConverter : IValueConverter {
        private static SKColor color = SKColors.Black;
        public object Convert(object val, Type targetType, object param, string lang) {
            if (val != null) {
                var vals = (val as List<double>);
                color = (vals[vals.Count - 1] > vals[0]) ?
                    SKColor.Parse(ColorConstants.GetColorBrush("pastel_green").Color.ToString()) :
                    SKColor.Parse(ColorConstants.GetColorBrush("pastel_red").Color.ToString());
                var entries = vals.ConvertAll(PointConverter);
                Chart chart = new LineChart() {
                    Entries = entries,
                    IsAnimated = false,
                    LineAreaAlpha = 0,
                    PointAreaAlpha = 0,
                    BackgroundColor = SKColors.Transparent,
                    LabelColor = SKColors.Green,
                    LineMode = LineMode.Straight,
                    LineSize = 1,
                    PointSize = 1,
                    MinValue = entries.Min(x => x.Value),
                    MaxValue = entries.Max(x => x.Value),
                };
                return chart;
            }
            else
                return new LineChart() {
                    Entries = new List<ChartEntry>() { new ChartEntry(0) },
                    BackgroundColor = SKColors.Transparent,
                    IsAnimated = false
                };
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();

        public static ChartEntry PointConverter(double num) {
            return new ChartEntry((float)num) { Color = color };
        }
    }
}
