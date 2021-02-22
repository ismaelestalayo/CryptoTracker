using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CryptoTracker.Helpers {
    static class Converters {

        public static string ShortenDateTime(DateTime date) => date.ToShortDateString();


        public static string ThousandSeparator(string number) => string.Format((string)number);

        public static string ToKMB(double num) {
            if (num > 999999999) {
                return num.ToString("0,,,.##B", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999999) {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            }
            else
            if (num > 999) {
                num = Math.Round(num, 2);
                return num.ToString(CultureInfo.InvariantCulture);
            }
            else {
                num = Math.Round(num, 3);
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }

    public class VisibilityInverter : IValueConverter {
        public object Convert(object value, Type targetType, object param, string language)  => (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        public object ConvertBack(object value, Type targetType, object param, string language) => (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }

    public class ListCountToVisibilityInvertedConverter : IValueConverter {
        public object Convert(object value, Type targetType, object param, string language) {
            return ((int)value) == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object param, string language) {
            return ((int)value) == 0 ? Visibility.Visible : Visibility.Collapsed;
        }
    }
    public class ListCountToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object param, string language) {
            return ((int)value) == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object param, string language) {
            return ((int)value) == 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public class StringFormatConverter : IValueConverter {
        public object Convert(object value, Type targetType, object param, string language) {
            try {
                var str = value.ToString();
                var num = double.Parse(str);
                if (!String.IsNullOrEmpty(str))
                    return num.ToString("N0", CultureInfo.CurrentCulture.NumberFormat);

                return value;
            }
            catch {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object param, string language) => throw new NotImplementedException();
    }

    public class DateTimeShortener : IValueConverter {
        public object Convert(object value, Type targetType, object param, string language) => ((DateTimeOffset)value).DateTime.ToShortDateString();
        public object ConvertBack(object value, Type targetType, object param, string language) => throw new NotImplementedException();
    }
}
