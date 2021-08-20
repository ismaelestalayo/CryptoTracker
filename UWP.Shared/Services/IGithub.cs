using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.Helpers;
using Windows.UI.Popups;

namespace UWP.Services {
    public interface IGithub {

		[Get("/CoinList.json")]
		Task<List<CmcCoin>> GetCmcCoins();

	}

    /// <summary>
    /// CoinMarketCap Coin
    /// </summary>
    public class CmcCoin {
        public int id { get; set; } = 0;
        public string name { get; set; } = "NULL";
        public string symbol { get; set; } = "NULL";
        public int rank { get; set; } = 0;
    }

    public static class GithubExtensions {
        public static async Task<List<CmcCoin>> GetCmcCoins_(this IGithub service) {
            try {
                var response = await service.GetCmcCoins();
                return response;
                //return JsonSerializer.Deserialize<List<CoinGeckoCoin>>(response.ToString());

                //coinList.Sort((x, y) => x.Symbol.CompareTo(y.Symbol));

                // Save on Local Storage, and save the Date
                await LocalStorageHelper.SaveObject("CoinList", response);
                //App._LocalSettings.Set(UserSettings.CoinListDate, DateTime.Today.ToOADate());

                return new List<CmcCoin>();
            }
            catch (Exception ex) {
                //return new List<CmcCoin>();
                await new MessageDialog(ex.Message).ShowAsync();
                return new List<CmcCoin>(){ new CmcCoin() {
                    id      = 1,
                    name    = "Erro",
                    symbol  = "ERR",
                    rank    = 1
                }};
            }
        }
    }
}
