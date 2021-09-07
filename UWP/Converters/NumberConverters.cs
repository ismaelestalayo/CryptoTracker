using System;
using UWP.Shared.Helpers;
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
            => NumberHelper.Rounder((double)val).ToString("N9", App.UserCulture).TrimEnd('0').Trim(',');
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
        public object Convert(object val, Type targetType, object param, string lang) {
            var num = (double)val;
            if (num > 999999999) {
                return num.ToString("0,,,.##B", App.UserCulture);
            }
            else if (num > 999999) {
                return num.ToString("0,,.##M", App.UserCulture);
            }
            else if (num > 999) {
                return num.ToString("0,.##K", App.UserCulture);
            }
            else {
                num = Math.Round(num, 3);
                return num.ToString(App.UserCulture);
            }
        }
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

    public class NumberSymbolPrefixer : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var z = new NumberRounder();
            double num = double.Parse(z.Convert(val, targetType, param, lang).ToString(), App.UserCulture);
            return string.Format("{0:▲0.#####;▼0.#####;}", (double)num);
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class IntToBoolConverter : IValueConverter {
        public bool Inverse { get; set; } = false;
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((int)val) == 0;
            else
                return ((int)val) != 0;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
