using System;
using UWP.Helpers;
using Windows.UI.Xaml.Data;

namespace UWP.Converters {
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

    public class PercentageRounder : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            double perc = Math.Round((double)val, 2);
            return new NumberSignPrefixer().Convert(perc, targetType, param, lang) + "%";
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberUnitSuffixer : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang)
            => NumberHelper.AddUnitPrefix((double)val);
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberSignPrefixer : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var z = new NumberRounder();
            double num = double.Parse(z.Convert(val, targetType, param, lang).ToString(), App.UserCulture);
            return string.Format("{0:+0.#####;-0.#####;}", (double)num);
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
