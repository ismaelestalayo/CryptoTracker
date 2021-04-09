using System.Collections.Generic;
using System.Linq;

namespace UWP.Shared.Constants {
    public class Currencies {
        public static string GetCurrencySymbol(string currency) {
            string symbol;
            return CurrencySymbol.TryGetValue(currency, out symbol) ? symbol : "€";
        }

        public static List<string> AllCurrencies => CurrencySymbol.Keys.ToList();

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
