using System;
using UWP.Shared.Helpers;
using Windows.UI.Xaml;
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

    public class NumberEqualTargetToBoolConverter : IValueConverter {
        public bool Inverse { get; set; }
        public int Target { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            if (Inverse)
                return ((int)val) != Target;
            else
                return ((int)val) == Target;
        }

        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberEqualTargetToVisibilityConverter : IValueConverter {
        public bool Inverse { get; set; }
        public int Target { get; set; }
        public object Convert(object val, Type targetType, object param, string lang) {
            var num = (int)val;

            if (Inverse)
                return (num != Target) ? Visibility.Visible : Visibility.Collapsed;
            else
                return (num == Target) ? Visibility.Visible : Visibility.Collapsed;
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
            var num = ((double)val).ToString("N5", App.UserCulture).TrimEnd('0').Trim(',');
            
            // Negative numbers already has the negative symbol
            if ((double)val < 0)
                return num;
            else
                return "+" + num;
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }

    public class NumberSymbolPrefixer : IValueConverter {
        public object Convert(object val, Type targetType, object param, string lang) {
            var num = ((double)val).ToString("N5", App.UserCulture).TrimEnd('0').Trim(',');
            var prefix = ((double)val >= 0) ? "▲ " : "▼ ";
            return prefix + num;
        }
        public object ConvertBack(object val, Type targetType, object param, string lang)
            => throw new NotImplementedException();
    }
}
