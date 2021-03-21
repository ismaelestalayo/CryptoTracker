using System.Collections.Generic;

namespace UWP.Shared.Constants {
    public class Currencies {
        public static string GetCurrencySymbol(string currency) {
            string symbol;
            return CurrencySymbol.TryGetValue(currency, out symbol) ? symbol : "€";
        }

        private static readonly Dictionary<string, string> CurrencySymbol = new Dictionary<string, string>() {
            { "AUD", "$" },
            { "BRL", "R$"},
            { "CAD", "$" },
            { "CNY", "¥" },
            { "EUR", "€" },
            { "GBP", "£" },
            { "INR", "₹" },
            { "JPY", "¥" },
            { "MXN", "$" },
            { "USD", "$" },
        };
    }
}
