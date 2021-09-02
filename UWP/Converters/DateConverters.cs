using System;
using Windows.UI.Xaml.Data;

namespace UWP.Converters {
    public class DateTimeFormatter : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => ((DateTime)val).ToString("dd MMM yyyy HH:mm");
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
            => ((DateTimeOffset)val).DateTime.ToString("MMM dd, yyy");
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
