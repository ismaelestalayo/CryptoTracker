using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace CryptoTracker.APIs {
	class CryptoCompare {

        internal async static Task<List<JSONcoin>> GetAllCoins() {
            bool UsedCache = false;
            List<JSONcoin> coinList = new List<JSONcoin>();
            
            Uri uri = new Uri("https://min-api.cryptocompare.com/data/all/coinlist?summary=true");

            try {
                var data = await App.GetJSONAsync(uri);
                coinList = JSONcoin.HandleJSON(data);
                coinList.Sort((x, y) => x.Symbol.CompareTo(y.Symbol));

                // Save on Local Storage, and save the Date
                LocalStorageHelper.SaveObject(coinList, "coinList");
                App.localSettings.Values["coinListDate"] = DateTime.Today.ToOADate();

                return coinList;
            }
            catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
                return new List<JSONcoin>(){ new JSONcoin() {
                    Symbol = "ERR",
                    FullName = "Error",
                    Id = 1,
                    ImageUrl = ""}
                };
            }
        }

    }
}
