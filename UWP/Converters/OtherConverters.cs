using System;
using UWP.Shared.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

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
}
