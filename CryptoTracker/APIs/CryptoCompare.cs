using System;

namespace CryptoTracker.APIs {
	class CryptoCompare {

        /* ###############################################################################################
         * Gets the current price of a coin (in the currency set by App.coin)
         * 
         * Arguments: 
         * - crypto (BTC, ETH...)
         * - market
        */
        internal static double GetPrice(string crypto, string market = "defaultMarket") {
            var currency = App.coin;
            string URL = string.Format("https://min-api.cryptocompare.com/data/price?fsym={0}&tsyms={1}", crypto, currency);

            if (market != "defaultMarket")
                URL += "&e=" + market;

            Uri uri = new Uri(URL);

            try {
                var data = App.GetStringAsync(uri).Result;

                double price = double.Parse(data.Split(":")[1].Replace("}", ""));

				if (price > 99)
					return Math.Round(price, 2);
				else if (price > 10)
					return Math.Round(price, 4);
                else
                    return Math.Round(price, 6);

            }
            catch (Exception) {
                return 0;
            }
        }

    }
}
