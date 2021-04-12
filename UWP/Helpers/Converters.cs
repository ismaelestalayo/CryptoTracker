using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UWP.Helpers {
    /// Numbers
    public class GeneralNumberConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((double)val).ToString("N5", App.UserCulture).TrimEnd('0').Trim(',');
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberRounder : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => NumberHelper.Rounder((double)val).ToString("N5", App.UserCulture).TrimEnd('0').Trim(',');
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberPrefixConverter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => NumberHelper.AddUnitPrefix((double)val);
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    /// Bool
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

    /// Dates
    public class DateTimeFormatter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((DateTime)val).ToString(App.UserCulture);
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class DateTimeShortener : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((DateTime)val).ToShortDateString();
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class DateTimeOffsetShortener : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((DateTimeOffset)val).DateTime.ToShortDateString();
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    /// Others
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
}
