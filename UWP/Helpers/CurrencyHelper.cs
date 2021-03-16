using System.Collections.Generic;

namespace UWP.Helpers {
    class CurrencyHelper {
        internal static string GetCurrencySymbol(string currency) {
            string symbol;
            return CurrencyToSymbol.TryGetValue(currency, out symbol) ? symbol : "€";
        }

        private static readonly Dictionary<string, string> CurrencyToSymbol = new Dictionary<string, string>() {
            { "AUD",  "$" },
            { "BRL", "R$"},
            { "CAD",  "$" },
            { "CNY",  "¥" },
            { "EUR",  "€" },
            { "GBP",  "£" },
            { "INR",  "₹" },
            { "JPY",  "¥" },
            { "MXN",  "$" },
            { "USD",  "$" },
        };
    }
}
