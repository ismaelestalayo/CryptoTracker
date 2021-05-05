using System.Collections.Generic;
using System.Linq;

namespace UWP.Shared.Constants {
    public class Currencies {
        public static string GetCurrencySymbol(string currency) {
            string symbol;
            return CurrencySymbol.TryGetValue(currency, out symbol) ? symbol : "€";
        }
        
        public static readonly Dictionary<string, string> CurrencySymbol = new Dictionary<string, string>() {
            { "ARS", "$" },
            { "AUD", "$" },
            { "BRL", "R$"},
            { "BTC", "₿"},
            { "CAD", "$" },
            { "CNY", "¥" },
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
