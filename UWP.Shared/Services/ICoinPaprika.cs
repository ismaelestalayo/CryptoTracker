using Refit;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Helpers;

namespace UWP.Services {
    public interface ICoinPaprika {

		[Get("/v1/tickers?quotes={quote}")]
		Task<string> GetTickers(string quote);


	}

    public static class CoinPaprikaExtensions {
        public static async Task<List<Ticker>> GetTickers_(this ICoinPaprika service, string quote) {
            var response = await service.GetTickers(quote);

            var data = JsonSerializer.Deserialize<List<Ticker>>(response.ToString());
            foreach (var d in data) {
                try {
                    var j = ((JsonElement)d.quotes).GetProperty(quote);
                    d.quote = JsonSerializer.Deserialize<Quote>(j.ToString());
                    d.logo = IconsHelper.GetIcon(d.symbol);
                }
                catch (Exception ex) {
                    d.quote = new Quote();
                }
            }

            return data;
        }
    }

    public class Ticker {
        public static string aaa = "EUR";
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int rank { get; set; }
        public double circulating_supply { get; set; }
        public double total_supply { get; set; }
        public double max_supply { get; set; }
        public double beta_value { get; set; }
        public string first_data_at { get; set; }
        public string last_updated { get; set; }
        public object quotes { get; set; }
        public Quote quote { get; set; }

        /// <summary>
        /// Manually added fields
        /// </summary>
        public string logo { get; set; }
    }

    public class Quote {
        public double price { get; set; }
        public double volume_24h { get; set; }
        public double volume_24h_change_24h { get; set; }
        public double market_cap { get; set; }
        public double market_cap_change_24h { get; set; }
        public double percent_change_15m { get; set; }
        public double percent_change_30m { get; set; }
        public double percent_change_1h { get; set; }
        public double percent_change_6h { get; set; }
        public double percent_change_12h { get; set; }
        public double percent_change_24h { get; set; }
        public double percent_change_7d { get; set; }
        public double percent_change_30d { get; set; }
        public double percent_change_1y { get; set; }
        public double ath_price { get; set; }
        public string ath_date { get; set; }
        public double percent_from_price_ath { get; set; }
    }
}
