using System;
using Telerik.UI.Xaml.Controls.Chart;
using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace UWP.Converters {
    public class NumberForeground : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (double)val >= 0 ? ColorConstants.GetColorBrush("pastel_green") : ColorConstants.GetColorBrush("pastel_red");
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class VisibilityInverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (Visibility)val == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    // Converts Visibility value to GridLineVisibility value for showing/hiding strip lines
    public class VisibilityToStripLinesConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (Visibility)val == Visibility.Visible ? GridLineVisibility.Y : GridLineVisibility.None;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class ListCountToVisibleConverter : IValueConverter {
        public bool Inverse { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((int)val) == 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return ((int)val) != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class DoubleToVisibilityConverter : IValueConverter {
        public bool Inverse { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((double)val) == 0 ? Visibility.Visible : Visibility.Collapsed;
            else
                return ((double)val) != 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BrushToColorTransparentConverter : IValueConverter {
        public int Transparency { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            var color = ((SolidColorBrush)val).Color;

            if (App._LocalSettings.Get<bool>(UserSettings.Minimal))
                return Color.FromArgb(0, color.R, color.G, color.B);

            return Color.FromArgb((byte)Transparency, color.R, color.G, color.B);
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BrushToColorConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var color = ((SolidColorBrush)val).Color;
            return Color.FromArgb(255, color.R, color.G, color.B);
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
