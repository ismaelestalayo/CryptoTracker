using System.Collections.Generic;
using System.Globalization;
using UWP.Core.Constants;
using UWP.Services;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media;

namespace UWP.Shared.Constants {
    public class ColorConstants {

        public static SolidColorBrush GetColorBrush(string key, int opacity = 255) {
            Color color;
            color = ColorDict.TryGetValue(key, out color) ? color : ColorDict["NULL_color"];

            if (opacity != 255)
                color.A = (byte)opacity;

            return new SolidColorBrush(color);
        }

        public static SolidColorBrush GetCoinBrush(string key, int opacity = 255) {
            Color color;
            color = ColorDict.TryGetValue(key, out color) ? color : ColorDict["NULL_color"];

            //var localSettings = new LocalSettings();
            //if (localSettings.Get<bool>(UserSettings.Monochrome)) {
            //    var darkTheme = CurrentThemeIsDark();
            //    color = (darkTheme) ? ParseHex("#f0f0f0") : ParseHex("#101010");
            //}

            if (opacity != 255)
                color.A = (byte)opacity;

            return new SolidColorBrush(color);
        }

        private static readonly Dictionary<string, Color> ColorDict = new Dictionary<string, Color>() {
            { "ADA_color",  ParseHex("#0033ad") },
            { "BAT_color",  ParseHex("#ff5000") },
            { "BCH_color",  ParseHex("#92c458") },
            { "BCN_color",  ParseHex("#FF3E89") },
            { "BNB_color",  ParseHex("#FFBB0C") },
            { "BTC_color",  ParseHex("#FFB119") },
            { "BTT_color",  ParseHex("#464750") },
            { "DASH_color", ParseHex("#0076C0") },
            { "DOGE_color", ParseHex("#CDA71F") },
            { "DOT_color",  ParseHex("#e50b7b") },
            { "EOS_color",  ParseHex("#443F54") },
            { "ETC_color",  ParseHex("#4C9172") },
            { "ETH_color",  ParseHex("#6F7CBA") },
            { "ICX_color",  ParseHex("#1aaaba") },
            { "IOT_color",  ParseHex("#04A997") },
            { "LINK_color", ParseHex("#295ada") },
            { "LTC_color",  ParseHex("#B5B5B5") },
            { "NANO_color", ParseHex("#0092E8") },
            { "NEO_color",  ParseHex("#58BF00") },
            { "ONT_color",  ParseHex("#00a6c0") },
            { "SOL_color",  ParseHex("#14f195") },
            { "UNI_color",  ParseHex("#ff007a") },
            { "USDT_color", ParseHex("#26A17B") },
            { "USDC_color", ParseHex("#0076ce") },
            { "THETA_color",ParseHex("#E60815") },
            { "TFUEL_color",ParseHex("#ff7c00") },
            { "TRX_color",  ParseHex("#E60815") },
            { "VET_color",  ParseHex("#15bdff") },
            { "XEM_color",  ParseHex("#64b3e1") },
            { "XLM_color",  ParseHex("#00B6E7") },
            { "XMR_color",  ParseHex("#EE5922") },
            { "XRP_color",  ParseHex("#1277A8") },
            { "XTZ_color",  ParseHex("#2c7df7") },
            { "ZRX_color",  ParseHex("#404040") },

            { "NULL_color",   ParseHex("#7F7F7F") },
            { "pastel_green", ParseHex("#00AD57") },
            { "pastel_red",   ParseHex("#FF5757") }
        };

        /// Color c = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#ABCDEF");
        private static Color ParseHex(string hexValue) => Color.FromArgb(
            byte.MaxValue,
            byte.Parse(hexValue.Substring(1, 2), NumberStyles.AllowHexSpecifier),
            byte.Parse(hexValue.Substring(3, 2), NumberStyles.AllowHexSpecifier),
            byte.Parse(hexValue.Substring(5, 2), NumberStyles.AllowHexSpecifier));

        /// <summary>
        /// Returns "light"/"dark" based on the current theme of the user
        /// </summary>
        public static bool CurrentThemeIsDark() {
            var localSettings = new LocalSettings();
            var theme = localSettings.Get<string>(UserSettings.Theme).ToLowerInvariant();
            if (theme == "light")
                return false;
            else if (theme == "dark")
                return true;
            bool isDark = new UISettings().GetColorValue(UIColorType.Background) == Colors.Black;
            return isDark;
        }
    }
}
