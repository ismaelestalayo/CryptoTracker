using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Services;
using Windows.UI.Popups;

namespace UWP.APIs {

    class GitHub {
        /*
         * Gets the list of coins and saves it under App.coinList
         * 
         * 
         * Returns: List<CoinBasicInfo>
         */
        internal async static Task<List<CoinBasicInfo>> GetAllCoins() {

            try {
                var coins = await Ioc.Default.GetService<IGithub>().GetAllCoins();

                //coinList.Sort((x, y) => x.Symbol.CompareTo(y.Symbol));

                // Save on Local Storage, and save the Date
                LocalStorageHelper.SaveObject("CoinList", coins);
                App._LocalSettings.Set(UserSettings.CoinListDate, DateTime.Today.ToOADate());

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
