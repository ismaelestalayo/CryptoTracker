using System.IO;

namespace CryptoTracker.Helpers {
    class IconsHelper {

        public static string GetIcon(string coin) {
            string filename = "Assets/Icons/icon" + coin + ".png";

            if (!File.Exists(filename))
                filename = "https://chasing-coins.com/coin/logo/" + coin;

            return filename;
        }
    }
}
