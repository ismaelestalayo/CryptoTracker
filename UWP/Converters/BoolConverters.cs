using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Converters {
    public class BoolToVisibility : IValueConverter {
        public bool Inverse { get; set; } = false;
        public object Convert(object val, Type targetType, object param, string lang) {
            var b = Inverse ? !(bool)val : (bool)val;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BoolIsLoadingToOpacity : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => (bool)val ? 0.33 : 1;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class BoolInverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => !(bool)val;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NullToBoolConverter : IValueConverter {
        public bool Inverse { get; set; } = false;

        public object Convert(object val, Type targetType, object param, string lang)
            => Inverse ? val != null : val == null;
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberEqualTargetToBoolConverter : IValueConverter {
        public bool Inverse { get; set; }
        public int Target { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((int)val) == Target;
            else
                return ((int)val) != Target;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberGreaterThanTargetToBoolConverter : IValueConverter {
        public bool Inverse { get; set; }
        public int Target { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((int)val) <= Target;
            else
                return ((int)val) > Target;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
