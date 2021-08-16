using System;

namespace UWP.Helpers {
    class NumberHelper {
        public static string AddUnitPrefix(double num) {
            if (num > 999999999) {
                return num.ToString("0,,,.##B", App.UserCulture);
            }
            else if (num > 999999) {
                return num.ToString("0,,.##M", App.UserCulture);
            }
            else if (num > 999) {
                num = Math.Round(num, 2);
                return num.ToString(App.UserCulture);
            }
            else {
                num = Math.Round(num, 3);
                return num.ToString(App.UserCulture);
            }
        }

        /// <summary>
        /// Round a double with a decimal precision that depends on its value
        /// </summary>
        public static double Rounder(double price) {
            if (Math.Abs(price) > 999)
                return Math.Round(price, 1);
            else if (Math.Abs(price) > 9)
                return Math.Round(price, 2);
            else if (Math.Abs(price) > 1)
                return Math.Round(price, 3);
            else if (Math.Abs(price) > 0.1)
                return Math.Round(price, 4);
            else
                return Math.Round(price, 5);
        }
    }
}
