using System;
using UWP.Shared.Constants;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP.Converters {
    public class NumberForeground : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (double)val >= 0 ? ColorConstants.GetBrush("pastel_green") : ColorConstants.GetBrush("pastel_red");
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class VisibilityInverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (Visibility)val == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class ListCountToVisibilityInvertedConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((int)val) == 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class ListCountToVisibilityConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((int)val) == 0 ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class DoubleToVisibilityConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((double)val) == 0 ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class DoubleToVisibilityInverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((double)val) != 0 ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BrushToColorTr1Converter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var color = ((SolidColorBrush)val).Color;
            return Color.FromArgb(64, color.R, color.G, color.B);
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BrushToColorTr2Converter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var color = ((SolidColorBrush)val).Color;
            return Color.FromArgb(8, color.R, color.G, color.B);
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
