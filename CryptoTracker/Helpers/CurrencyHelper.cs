using System;

namespace CryptoTracker.Helpers {
    class CurrencyHelper {

        public static string CurrencyToSymbol(string currency) {

            switch (currency) {
                case "EUR":
                    return "€";
                case "GBP":
                    return "£";
                case "USD":
                case "CAD":
                case "AUD":
                case "MXN":
                    return "$";
                case "CNY":
                case "JPY":
                    return "¥";
                case "INR":
                    return "₹";
                default:
                    return "ERR";
            }
        }
    }
}
