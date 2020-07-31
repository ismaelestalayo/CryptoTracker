using Windows.UI.Xaml;
using System;
using Windows.UI.Xaml.Data;
using System.Globalization;

namespace CryptoTracker.Helpers {
    static class Converters {
        public static Visibility InvertVisibility(Visibility value) => value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

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

    public class StringFormatConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
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

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class DateTimeShortener : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) => ((DateTimeOffset)value).DateTime.ToShortDateString();
        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
