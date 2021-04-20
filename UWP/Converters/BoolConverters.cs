using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Converters {
    public class BoolToVisibility : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((bool)val) ? Visibility.Visible : Visibility.Collapsed;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
    public class BoolInverseToVisibility : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((bool)val) ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
