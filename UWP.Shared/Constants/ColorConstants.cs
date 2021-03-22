using System.Collections.Generic;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.Shared.Constants {
    public class ColorConstants {

        public static SolidColorBrush GetBrush(string key, int opacity = 255) {
            Color color;
            bool semiTranparent = false;

            //if (key.EndsWith("_colorT")) {
            //    key = key.Replace("_colorT", "_color");
            //    semiTranparent = true;
            //}

            color = ColorDict.TryGetValue(key, out color) ? color : ColorDict["NULL_color"];

            if (opacity != 255)
                color.A = (byte)opacity;

            return new SolidColorBrush(color);
        }

        private static readonly Dictionary<string, Color> ColorDict = new Dictionary<string, Color>() {
            { "ADA_color",  ParseHex("#091a26") },
            { "BCH_color",  ParseHex("#F08B16") },
            { "BCN_color",  ParseHex("#FF3E89") },
            { "BNB_color",  ParseHex("#FFBB0C") },
            { "BTC_color",  ParseHex("#FFB119") },
            { "BTT_color",  ParseHex("#101010") },
            { "DASH_color", ParseHex("#0076C0") },
            { "DOGE_color", ParseHex("#CDA71F") },
            { "DOT_color",  ParseHex("#e50b7b") },
            { "EOS_color",  ParseHex("#443F54") },
            { "ETC_color",  ParseHex("#4C9172") },
            { "ETH_color",  ParseHex("#6F7CBA") },
            { "ICX_color",  ParseHex("#1aaaba") },
            { "IOT_color",  ParseHex("#04A997") },
            { "LTC_color",  ParseHex("#B5B5B5") },
            { "NANO_color", ParseHex("#0092E8") },
            { "NEO_color",  ParseHex("#58BF00") },
            { "ONT_color",  ParseHex("#00a6c0") },
            { "USDT_color", ParseHex("#26A17B") },
            { "USDC_color", ParseHex("#0076ce") },
            { "TRX_color",  ParseHex("#E60815") },
            { "XEM_color",  ParseHex("#20B5AC") },
            { "XLM_color",  ParseHex("#00B6E7") },
            { "XMR_color",  ParseHex("#EE5922") },
            { "XRP_color",  ParseHex("#1277A8") },
            { "ZRX_color",  ParseHex("#131313") },

            { "NULL_color",  ParseHex("#7D7D7D") }
        };

        /// Color c = (Color)XamlBindingHelper.ConvertValue(typeof(Color), "#ABCDEF");
        private static Color ParseHex(string hexValue) => Color.FromArgb(
            byte.MaxValue,
            byte.Parse(hexValue.Substring(1, 2), NumberStyles.AllowHexSpecifier),
            byte.Parse(hexValue.Substring(3, 2), NumberStyles.AllowHexSpecifier),
            byte.Parse(hexValue.Substring(5, 2), NumberStyles.AllowHexSpecifier));
    }
}
