using UWP.Core.Constants;
using UWP.Helpers;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace UWP.APIs {
	public class CoinBasicInfo {
        public int id { get; set; } = 0;
        public string name { get; set; } = "NULL";
        public string symbol { get; set; } = "NULL";
        public int rank { get; set; } = 0;
    }

    class GitHub {
        /*
         * Gets the list of coins and saves it under App.coinList
         * 
         * 
         * Returns: List<CoinBasicInfo>
         */
        internal async static Task<List<CoinBasicInfo>> GetAllCoins() {

            Uri uri = new Uri("https://raw.githubusercontent.com/ismaelestalayo/CryptoTracker/API/CoinList.json");

            try {
                var data = await App.GetStringAsync(uri);

                var coins = JsonSerializer.Deserialize<List<CoinBasicInfo>>(data);

                //coinList.Sort((x, y) => x.Symbol.CompareTo(y.Symbol));

                // Save on Local Storage, and save the Date
                LocalStorageHelper.SaveObject(coins, "CoinList");
                App._LocalSettings.Set(UserSettingsConstants.CoinListDate, DateTime.Today.ToOADate());

                return coins;
            }
            catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
                return new List<CoinBasicInfo>(){ new CoinBasicInfo() {
                    id      = 1,
                    name    = "Erro",
                    symbol  = "ERR",
                    rank    = 1
                }};
            }
        }
    }
}
