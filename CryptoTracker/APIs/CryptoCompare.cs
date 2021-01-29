using CryptoTracker.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoTracker.APIs {
	class CryptoCompare {

        public class CryptoCompareResponse {
            public string Response { get; set; }
            public int Type { get; set; }
            public bool Aggregated { get; set; }
            public bool FirstValueInArray { get; set; }
            public double TimeTo { get; set; }
            public double TimeFrom { get; set; }
            public object ConversionType { get; set; }
            public object Data { get; set; }
        }

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

        /* ###############################################################################################
         * Gets the current price of a coin (in the currency set by App.coin)
         * 
         * Arguments: 
         * - crypto (BTC, ETH...)
         * - market
        */
        internal static async void GetHistoric(string crypto, string time, int limit) {
            var coin = App.coin;

            //CCCAGG Bitstamp Bitfinex Coinbase HitBTC Kraken Poloniex 
            string URL = string.Format("https://min-api.cryptocompare.com/data/histo{0}?e=CCCAGG&fsym={1}&tsym={2}&limit={3}", time, crypto, coin, limit);

            if (limit == 0)
                URL = string.Format("https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym={0}&tsym={1}&allData=true", crypto, coin);

            Uri uri = new Uri(URL);

            try {
                var data = await App.GetStringAsync(uri);
                //var data = JToken.Parse(response);
                var z = new List<JSONhistoric>();
                var response = JsonSerializer.Deserialize<CryptoCompareResponse>(data);

                var r = JsonSerializer.Deserialize<object>(data);
                var d = ((JsonElement)r).GetProperty("Data");
            }
            catch (Exception ex) {
                var dontWait = ex;
            }
        }
    }
}
