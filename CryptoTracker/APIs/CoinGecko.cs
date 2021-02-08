using CryptoTracker.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoTracker.APIs {
	class CoinGecko {

        internal async static Task<GlobalStats> GetGlobalStats() {
            var URL = "https://api.coingecko.com/api/v3/global";

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                var response = JsonSerializer.Deserialize<object>(responseString);

                var data = ((JsonElement)response).GetProperty("data");

                var stats = new GlobalStats();
                var currency = App.currency.ToLowerInvariant();
                var btcDominance = double.Parse(
                    data.GetProperty("market_cap_percentage").GetProperty("btc").ToString()
                );
                var totalVolume = double.Parse(
                    data.GetProperty("total_volume").GetProperty(currency).ToString()
                );
                var totalMarketCap = double.Parse(
                    data.GetProperty("total_market_cap").GetProperty(currency).ToString()
                );

                stats.BtcDominance = Math.Round(btcDominance, 2);
                stats.TotalVolume = App.ToKMB(totalVolume);
                stats.TotalMarketCap = App.ToKMB(totalMarketCap);
                stats.CurrencySymbol = App.currencySymbol;
                return stats;

            }
            catch (Exception) {
                //await new MessageDialog(ex.Message).ShowAsync();
                return new GlobalStats();
            }
        }
    }
}
