using Refit;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace UWP.Services {
    public interface ICoinPaprika {

		[Get("/v1/tickers?quotes={quote}")]
		Task<string> GetTickers(string quote);

        [Get("/v1/coins")]
        Task<string> GetCoinList();

        [Get("/v1/coins/{crypto}/ohlcv/historical")]
        Task<string> GetOHLCV(string crypto);
    }

    public static class CoinPaprikaExtensions {
        public static async Task<List<CoinPaprikaCoin>> GetCoinList_(this ICoinPaprika service) {
            try {
                var response = await service.GetCoinList();
                return JsonSerializer.Deserialize<List<CoinPaprikaCoin>>(response.ToString());
            }
            catch (Exception ex) {
                return new List<CoinPaprikaCoin>();
            }
        }
    }

    public class CoinPaprikaCoin {
        public string id { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public int rank { get; set; } = 0;
        public string type { get; set; }
    }
}
