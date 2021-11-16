using System.Collections.Generic;

namespace UWP.Shared.Constants {
    public class Currencies {
        public static string GetCurrencySymbol(string currency = "") {
            string symbol;
            return CurrencySymbol.TryGetValue(currency, out symbol) ? symbol : " ";
        }

        /// TODO: Once I add a good way to convert currencies, add the following:
        /// { "PHP", "₱"},

        public static readonly Dictionary<string, string> CurrencySymbol = new Dictionary<string, string>() {
            { "ARS", "$" },
            { "AUD", "$" },
            { "BRL", "R$"},
            { "BTC", "₿"},
            { "CAD", "$" },
            { "CNY", "¥" },
            { "CZK", "Kč" },
            { "ETH", "Ξ" },
            { "EUR", "€" },
            { "GBP", "£" },
            { "ILS", "₪" },
            { "INR", "₹" },
            { "JPY", "¥" },
            { "MXN", "$" },
            { "PLN", "zł"},
            { "USD", "$" },
        };
    }
}
