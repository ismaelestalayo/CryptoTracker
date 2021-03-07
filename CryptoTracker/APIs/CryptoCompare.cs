using CryptoTracker.Helpers;
using CryptoTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.APIs {
	class CryptoCompare {

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
        internal static async Task<List<HistoricPrice>> GetHistoricAsync(string crypto, string time, int limit, int aggregate = 1) {
            var currency = App.currency;
            var NullValue = new List<HistoricPrice>() { new HistoricPrice() { Average = 1, DateTime = DateTime.Today } };

            string URL = string.Format("https://min-api.cryptocompare.com/data/histo{0}?e=CCCAGG&fsym={1}&tsym={2}&limit={3}", time, crypto, currency, limit);

            if (aggregate != 1)
                URL += string.Format("&aggregate={0}", aggregate);

            if (limit == 0)
                URL = string.Format("https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym={0}&tsym={1}&allData=true", crypto, currency);

            
            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                var response = JsonSerializer.Deserialize<object>(responseString);

                var okey = ((JsonElement)response).GetProperty("Response").ToString();

                if (okey != "Success")
                    return NullValue;
                
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
                
                // if getting all history, remove null prices
                if (limit == 0) {
                    int i = historic.FindIndex(x => x.Average != 0);
                    if (i != 0)
                        historic.RemoveRange(0, i - 1);
                }

                    //historic = historic.FindAll(x => x.Average != 0);

                return historic;
            }
            catch (Exception ex) {
                return NullValue;
            }
        }

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
            var top100 = new List<Top100card>();

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                var response = JsonSerializer.Deserialize<object>(responseString);

                var data = ((JsonElement)response).GetProperty("Data");

                for (int i = 0; i < limit; i++) {
                    var _coinInfo = data[i].GetProperty("CoinInfo");
                    var coinInfo = JsonSerializer.Deserialize<CoinInfo>(_coinInfo.ToString());

                    Raw raw = new Raw();
                    var rawExists = data[i].TryGetProperty("RAW", out var _raw);
                    if (rawExists) {
                        _raw = _raw.GetProperty(currency.ToUpperInvariant());
                        raw = JsonSerializer.Deserialize<Raw>(_raw.ToString());
                    }

                    

                    /// quick fixes
                    coinInfo.ImageUrl = IconsHelper.GetIcon(coinInfo.Name);
                    coinInfo.Rank = i + 1;
                    coinInfo.FavIcon = App.pinnedCoins.Contains(coinInfo.Name) ? "\uEB52" : "\uEB51";
                    coinInfo.ChangeFG = (raw.CHANGE24HOUR < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
                    coinInfo.MarketCap = NumberHelper.AddUnitPrefix(raw.MKTCAP);
                    coinInfo.Volume = NumberHelper.AddUnitPrefix(raw.TOTALVOLUME24HTO);
                    raw.CHANGEPCT24HOUR = NumberHelper.Rounder(raw.CHANGEPCT24HOUR);
                    raw.CHANGE24HOUR = NumberHelper.Rounder(raw.CHANGE24HOUR);
                    raw.PRICE = NumberHelper.Rounder(raw.PRICE);

                    top100.Add(new Top100card() {
                        CoinInfo = coinInfo,
                        Raw = raw
                    });
                }
                return top100;
            }
            catch (Exception ex) {
                return top100;
            }
        }

        /* ###############################################################################################
         * Gets the top 100 coins (by marketcap)
         * 
         * Arguments: none
         * 
        */
        internal async static Task<Raw> GetCoinStats(string crypto) {
            var currency = App.currency.ToUpperInvariant();
            crypto = crypto.ToUpperInvariant();

            var URL = string.Format("https://min-api.cryptocompare.com/data/pricemultifull?fsyms={0}&tsyms={1}&e=CCCAGG",
                crypto, currency);

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                var response = JsonSerializer.Deserialize<object>(responseString);
                var data = ((JsonElement)response).GetProperty("RAW").GetProperty(crypto).GetProperty(currency);
                var raw = JsonSerializer.Deserialize<Raw>(data.ToString());

                /// quick fixes
                raw.PRICE = NumberHelper.Rounder(raw.PRICE);
                raw.CHANGEPCT24HOUR = NumberHelper.Rounder(raw.CHANGEPCT24HOUR);
                raw.CHANGE24HOUR = NumberHelper.Rounder(raw.CHANGE24HOUR);
                raw.PRICE = NumberHelper.Rounder(raw.PRICE);
                
                return raw;
            }
            catch (Exception ex) {
                return new Raw();
            }
        }

        /* ###############################################################################################
         * Gets the latest news or its categories
         * 
         * Arguments:
         * - filters: 
         * 
        */
        internal async static Task<List<NewsData>> GetNews(List<string> filters) {
            string URL = "https://min-api.cryptocompare.com/data/v2/news/?lang=EN";
            if (filters.Count > 0)
                URL += string.Format("&categories={0}", string.Join(",", filters));

            try {
                var responseString = await App.GetStringFromUrlAsync(URL);

                var news = JsonSerializer.Deserialize<NewsResponse>(responseString);
                foreach (NewsData n in news.Data) {
                    n.categorylist = n.categories.Split('|').ToList();
                    if (n.categorylist.Count > 3)
                        n.categorylist = n.categorylist.GetRange(1, 3);
                }
                return news.Data;
            }
            catch (Exception ex) {
                return new List<NewsData>();
            }
        }

        internal async static Task<List<NewsCategories>> GetNewsCategories() {
            string URL = "https://min-api.cryptocompare.com/data/news/categories";

            List<NewsCategories> categories;
            try {
                var responseString = await App.GetStringFromUrlAsync(URL);
                return JsonSerializer.Deserialize<List<NewsCategories>>(responseString);
            }
            catch (Exception ex) {
                return new List<NewsCategories>() { new NewsCategories() };
            }
        }

    }
}
