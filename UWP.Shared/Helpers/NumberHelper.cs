using System;

namespace UWP.Shared.Helpers {
    public class NumberHelper {
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
            else if (Math.Abs(price) > 0.01)
                return Math.Round(price, 5);
            else
                return Math.Round(price, 8);
        }
    }
}
