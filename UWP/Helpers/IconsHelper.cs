using System.IO;

namespace CryptoTracker.Helpers {
    class IconsHelper {

        public static string GetIcon(string coin) {
            string filename = "Assets/Icons/icon" + coin + ".png";

            if (!File.Exists(filename))
                return "https://chasing-coins.com/api/v1/std/logo/" + coin;
            else
                return "/" + filename;
        }
    }
}
