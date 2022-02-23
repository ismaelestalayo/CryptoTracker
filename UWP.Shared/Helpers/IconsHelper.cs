using System;
using System.IO;

namespace UWP.Helpers {
    public class IconsHelper {

        /// <summary>
        /// Get a coin's icon (either from local Asset or from internet)
        /// </summary>
        /// Old endpoint: chasing-coins.com
        public static string GetIcon(string coin) {
            string filename = $"Assets/Icons/icon{coin}.png";

            try {
                if (!File.Exists(filename))
                    return $"https://cdn.jsdelivr.net/gh/atomiclabs/cryptocurrency-icons@d5c68edec1f5eaec59ac77ff2b48144679cebca1/128/color/{coin.ToLowerInvariant()}.png";
                else
                    return $"/Assets/Icons/icon{coin}.png";
            }
            catch (Exception ex) {
                return "/Assets/Icons/iconNULL.png";
            }
        }
    }
}
