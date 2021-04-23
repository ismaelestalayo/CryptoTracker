using Refit;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Helpers;

namespace UWP.Services {
    public interface ICoinGecko {

		[Get("/global")]
		Task<string> GetGlobalStats();

		[Get("/coins/{coin}?localization=false&tickers=false&community_data=false&developer_data=false")]
		Task<string> GetCoin(string coin);

		[Get("/coins/markets?vs_currency={currency}&order=market_cap_desc&per_page=250&page=1&sparkline=true&price_change_percentage=1h,24h,7d,30d,1y")]
		Task<string> GetCoinsMarkets(string currency);
	}

	public static class CoinGeckoExtensions {
		public static async Task<List<CoinMarket>> GetCoinsMarkets_(this ICoinGecko service, string currency) {
			var response = await service.GetCoinsMarkets(currency);

			var data = JsonSerializer.Deserialize<List<CoinMarket>>(response.ToString());
            foreach (var d in data) {
                var z = ((JsonElement)d.sparkline_in_7d).GetProperty("price");
                d.sparkline_7d = JsonSerializer.Deserialize<List<double>>(z.ToString());
                var img = IconsHelper.GetIcon(d.symbol.ToUpperInvariant());
                d.image = img.StartsWith("/Assets") ? img : d.image;
            }
            return data;
		}
	}

    public class CoinMarket {
        public string id { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public double current_price { get; set; } = 0;
        public double market_cap { get; set; } = 0;
        public int market_cap_rank { get; set; }
        public double total_volume { get; set; } = 0;
        public double high_24h { get; set; } = 0;
        public double low_24h { get; set; } = 0;
        public double price_change_24h { get; set; } = 0;
        public double price_change_percentage_24h { get; set; } = 0;
        public double market_cap_change_24h { get; set; } = 0;
        public double market_cap_change_percentage_24h { get; set; } = 0;
        public double circulating_supply { get; set; } = 0;
        public double? total_supply { get; set; } = 0;
        public double? max_supply { get; set; } = 0;
        public double ath { get; set; } = 0;
        public string ath_date { get; set; }
        public double atl { get; set; } = 0;
        public string atl_date { get; set; }
        public List<double> sparkline_7d { get; set; }
        public object sparkline_in_7d { get; set; }
        public double? price_change_percentage_1h_in_currency { get; set; } = 0;
        public double? price_change_percentage_24h_in_currency { get; set; } = 0;
        public double? price_change_percentage_30d_in_currency { get; set; } = 0;
        public double? price_change_percentage_7d_in_currency { get; set; } = 0;
        public double? price_change_percentage_1y_in_currency { get; set; } = 0;
    }
}
