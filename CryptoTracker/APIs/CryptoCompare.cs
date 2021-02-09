using CryptoTracker.Helpers;
using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoTracker.APIs {
	class CryptoCompare {

        public class HistoricPrice {
            public int time { get; set; }
            public float high { get; set; } = 0;
            public float low { get; set; } = 0;
            public float open { get; set; } = 0;
            public float close { get; set; } = 0;
            public float volumefrom { get; set; } = 0;
            public float volumeto { get; set; } = 0;

            internal float Average { get; set; } = 0;
            internal string Date { get; set; }
            internal DateTime DateTime { get; set; }
        }

        /* ###############################################################################################
         * Gets the current price of a coin (in the currency set by App.currency)
         * 
         * Arguments: 
         * - crypto: BTC ETH...
         * - market: CCCAGG Bitstamp Bitfinex Coinbase HitBTC Kraken Poloniex
         * 
        */

        internal static async Task<double> GetPriceAsync(string crypto, string market = "null") {
            var currency = App.currency;
            string URL = string.Format("https://min-api.cryptocompare.com/data/price?fsym={0}&tsyms={1}", crypto, currency);

            if (market != "null")
                URL += "&e=" + market;

            Uri uri = new Uri(URL);

            try {
                var data = await App.GetStringAsync(uri);
                double price = double.Parse(data.Split(":")[1].Replace("}", ""));
                return NumberHelper.Rounder(price);
            }
            catch (Exception) {
                return 0;
            }
        }

        /* ###############################################################################################
         * Gets the current price of a coin (in the currency set by App.currency)
         * 
         * Arguments: 
         * - crypto: BTC ETH...
         * - time: minute hour day
         * - limit: 1 - 2000
         * 
        */
        internal static async Task<List<HistoricPrice>> GetHistoricAsync(string crypto, string time, int limit) {
            var currency = App.currency;

            string URL = string.Format("https://min-api.cryptocompare.com/data/histo{0}?e=CCCAGG&fsym={1}&tsym={2}&limit={3}", time, crypto, currency, limit);

            if (limit == 0)
                URL = string.Format("https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym={0}&tsym={1}&allData=true", crypto, currency);

            
            try {
                var responseString = await App.GetStringAsync(new Uri(URL));

                var response = JsonSerializer.Deserialize<object>(responseString);

                var okey = ((JsonElement)response).GetProperty("Response").ToString();

                if (okey != "Success")
                    return new List<HistoricPrice>() { new HistoricPrice() };
                
                var data = ((JsonElement)response).GetProperty("Data").ToString();
                var historic = JsonSerializer.Deserialize<List<HistoricPrice>>(data);

                // Add calculation of dates and average values
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                foreach (var h in historic) {
                    h.Average = (h.high + h.low) / 2;
                    DateTime d = dtDateTime.AddSeconds(h.time).ToLocalTime();
                    h.DateTime = d;
                    h.Date = d.ToString();
                }

                return historic;
            }
            catch (Exception ex) {
                return new List<HistoricPrice>(3);
            }
        }


        /* ###############################################################################################
         * Gets the exchanges for a crypto (with the price and volume)
         * TODO: unused endpoint
         * 
         * Arguments: 
         * - crypto: BTC ETH...
         * 
        */
        internal async static Task GetExchanges(string crypto) {
            var currency = App.currency;

            var URL = string.Format("https://min-api.cryptocompare.com/data/top/exchanges?fsym={0}&tsym={1}&limit={2}", crypto, currency, 8);

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);

                var response = JsonSerializer.Deserialize<object>(responseString);

                var okey = ((JsonElement)response).GetProperty("Response").ToString();

                var data = ((JsonElement)response).GetProperty("Data").ToString();
                var exchanges = JsonSerializer.Deserialize<List<Exchange>>(data);
            }
            catch (Exception ex) {
                var message = ex.Message;
            }
        }

        public class Exchange {
            public string exchange { get; set; } = "NULL";
            public string fromSymbol { get; set; } = "NULL";
            public string toSymbol { get; set; } = "NULL";
            public double volume24h { get; set; } = 0;
            public double volume24hTo { get; set; } = 0;
            public double price { get; set; } = 0;
            public string exchangeGrade { get; set; } = "null";
        }

        /* ###############################################################################################
         * Gets the top 100 coins (by marketcap)
         * 
         * Arguments: none
         * 
        */
        internal async static Task<List<Top100card>> GetTop100() {
            int limit = 100;
            var currency = App.currency;

            var URL = string.Format("https://min-api.cryptocompare.com/data/top/totalvolfull?tsym={0}&limit={1}", currency, limit);

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                var response = JsonSerializer.Deserialize<object>(responseString);

                var data = ((JsonElement)response).GetProperty("Data");

                var top100 = new List<Top100card>();
				for (int i = 0; i < limit; i++) {
                    var coinInfo = data[i].GetProperty("CoinInfo");
                    var rawExists = data[i].TryGetProperty("RAW", out var raw);
                    if (rawExists)
                        raw = raw.GetProperty(currency.ToUpperInvariant());
                    else
                        raw = new JsonElement();

                    top100.Add(new Top100card() {
                        CoinInfo = JsonSerializer.Deserialize<CoinInfo>(coinInfo.ToString()),
                        Raw = JsonSerializer.Deserialize<Raw>(raw.ToString())
                    });
                }

				for (int i = 0; i < top100.Count; i++) {
                    var coin = top100[i];
                    coin.CoinInfo.FavIcon = App.pinnedCoins.Contains(coin.CoinInfo.Name) ? "\uEB52" : "\uEB51";
                    coin.CoinInfo.Rank = i;
                    coin.CoinInfo.IconSrc = IconsHelper.GetIcon(coin.CoinInfo.Name);
                }
				foreach (var coin in top100) {
                    int i = top100.IndexOf(coin);
                    coin.CoinInfo.FavIcon = App.pinnedCoins.Contains(coin.CoinInfo.Name) ? "\uEB52" : "\uEB51";
                    coin.CoinInfo.Rank = i;
                    coin.CoinInfo.IconSrc = IconsHelper.GetIcon(coin.CoinInfo.Name);
                }

                return top100;

            }
            catch (Exception ex) {
                return new List<Top100card>();
            }
        }
    }
}
