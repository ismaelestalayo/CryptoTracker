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
            color = ColorDict.TryGetValue(key, out color) ? color : ColorDict["coin_NULL"];

            if (opacity != 255)
                color.A = (byte)opacity;

            return new SolidColorBrush(color);
        }

        public static SolidColorBrush GetCoinBrush(string crypto, int opacity = 255) {
            Color color;
            crypto = crypto.ToUpperInvariant();

            color = ColorDict.TryGetValue($"coin_{crypto}", out color) ? color : ColorDict["coin_NULL"];

            var localSettings = new LocalSettings();
            if (localSettings.Get<bool>(UserSettings.Monochrome)) {
                var darkTheme = CurrentThemeIsDark();
                color = darkTheme ? ParseHex("#f0f0f0") : ParseHex("#101010");
            }

            if (opacity != 255)
                color.A = (byte)opacity;

            return new SolidColorBrush(color);
        }

        private static readonly Dictionary<string, Color> ColorDict = new Dictionary<string, Color>() {
            { "coin_ADA",   ParseHex("#0033ad") },
            { "coin_ALGO",  ParseHex("#606060") },
            { "coin_AMPL",  ParseHex("#606060") },
            { "coin_AVAX",  ParseHex("#ff3f40") },
            { "coin_BAT",   ParseHex("#ff5000") },
            { "coin_BCH",   ParseHex("#92c458") },
            { "coin_BCN",   ParseHex("#FF3E89") },
            { "coin_BNB",   ParseHex("#ffbc00") },
            { "coin_BTC",   ParseHex("#FFB119") },
            { "coin_BTT",   ParseHex("#464750") },
            { "coin_CELO",  ParseHex("#fbcc5c") },
            { "coin_COMP",  ParseHex("#00d395") },
            { "coin_DASH",  ParseHex("#0076C0") },
            { "coin_DGB",   ParseHex("#006ad2") },
            { "coin_DOGE",  ParseHex("#CDA71F") },
            { "coin_DOT",   ParseHex("#e6007a") },
            { "coin_EOS",   ParseHex("#443F54") },
            { "coin_ETC",   ParseHex("#4C9172") },
            { "coin_ETH",   ParseHex("#6F7CBA") },
            { "coin_FIL",   ParseHex("#0090ff") },
            { "coin_FLOW",  ParseHex("#00ee88") },
            { "coin_GRT",   ParseHex("#5942cc") },
            { "coin_ICX",   ParseHex("#1aaaba") },
            { "coin_IOT",   ParseHex("#04A997") },
            { "coin_KSM",   ParseHex("#e6007a") },
            { "coin_LINK",  ParseHex("#295ada") },
            { "coin_LTC",   ParseHex("#B5B5B5") },
            { "coin_MATIC", ParseHex("#6f41d8") },
            { "coin_NANO",  ParseHex("#0092E8") },
            { "coin_NEO",   ParseHex("#58BF00") },
            { "coin_NU",    ParseHex("#1e65f3") },
            { "coin_ONT",   ParseHex("#00a6c0") },
            { "coin_QTUM",  ParseHex("#2e9ad0") },
            { "coin_SHIB",  ParseHex("#ff9300") },
            { "coin_SOL",   ParseHex("#14f195") },
            { "coin_UNI",   ParseHex("#ff007a") },
            { "coin_USDT",  ParseHex("#26A17B") },
            { "coin_USDC",  ParseHex("#0076ce") },
            { "coin_THETA", ParseHex("#E60815") },
            { "coin_TFUEL", ParseHex("#ff7c00") },
            { "coin_TRX",   ParseHex("#ef0027") },
            { "coin_VET",   ParseHex("#15bdff") },
            { "coin_XEM",   ParseHex("#64b3e1") },
            { "coin_XLM",   ParseHex("#00B6E7") },
            { "coin_XMR",   ParseHex("#EE5922") },
            { "coin_XRP",   ParseHex("#1277A8") },
            { "coin_XTZ",   ParseHex("#2c7df7") },
            { "coin_ZRX",   ParseHex("#606060") },

            { "coin_NULL",  ParseHex("#7F7F7F") },

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
