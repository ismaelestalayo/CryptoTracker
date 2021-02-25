using System;
using System.Collections.Generic;

namespace CryptoTracker.Helpers {
    class CurrencyHelper {
        internal static string GetCurrencySymbol(string currency) {
            string symbol;
            return CurrencyToSymbol.TryGetValue(currency, out symbol) ? symbol : "€";
        }

        private static readonly Dictionary<string, string> CurrencyToSymbol = new Dictionary<string, string>() {
            { "EUR",  "€" },
            { "GBP",  "$" },
            { "USD",  "$" },
            { "CAD",  "$" },
            { "AUD",  "$" },
            { "MXN",  "$" },
            { "CNY",  "¥" },
            { "JPY",  "¥" },
            { "INR",  "₹" },
        };
    }
}
