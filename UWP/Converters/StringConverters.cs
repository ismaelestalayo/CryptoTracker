using System;
using System.Globalization;
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
}
