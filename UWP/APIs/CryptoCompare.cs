using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using Windows.UI.Xaml.Media;

namespace UWP.APIs {
    class CryptoCompare {

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

            try {
                var resp = await Ioc.Default.GetService<ICryptoCompare>().GetExchanges(crypto, currency, 8);
                var response = JsonSerializer.Deserialize<object>(resp.ToString());

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

            var top100 = new List<Top100card>();

            try {
                var resp = await Ioc.Default.GetService<ICryptoCompare>().GetTop100(currency, limit);
                var response = JsonSerializer.Deserialize<object>(resp.ToString());

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
                    coinInfo.FavIcon = App.pinnedCoins.Contains(coinInfo.Name) ? "\uEB52" : "\uEB51";
                    coinInfo.ChangeFG = (raw.CHANGE24HOUR < 0) ? (SolidColorBrush)App.Current.Resources["pastelRed"] : (SolidColorBrush)App.Current.Resources["pastelGreen"];
                    //coinInfo.MarketCap = NumberHelper.AddUnitPrefix(raw.MKTCAP);
                    //coinInfo.Volume = NumberHelper.AddUnitPrefix(raw.TOTALVOLUME24HTO);
                    //raw.CHANGEPCT24HOUR = Math.Round(raw.CHANGEPCT24HOUR, 2);
                    //raw.CHANGE24HOUR = NumberHelper.Rounder(raw.CHANGE24HOUR);
                    //raw.PRICE = NumberHelper.Rounder(raw.PRICE);

                    top100.Add(new Top100card() {
                        CoinInfo = coinInfo,
                        Raw = raw
                    });
                }
                top100.Sort((x, y) => y.Raw.MKTCAP.CompareTo(x.Raw.MKTCAP));
                for (int i = 0; i < limit; i++)
                    top100[i].CoinInfo.Rank = i;
                return top100;
            }
            catch (Exception ex) {
                return top100;
            }
        }

        /* ###############################################################################################
         * Gets a coin's stats
         * TODO: unused endpoint
         * 
         * Arguments: none
         * 
        */
        internal async static Task<Raw> GetCoinStats(string crypto) {
            var currency = App.currency.ToUpperInvariant();
            crypto = crypto.ToUpperInvariant();

            try {
                var resp = await Ioc.Default.GetService<ICryptoCompare>().GetCoinStats(crypto, currency);
                var response = JsonSerializer.Deserialize<object>(resp.ToString());

                var data = ((JsonElement)response).GetProperty("RAW").GetProperty(crypto).GetProperty(currency);
                var raw = JsonSerializer.Deserialize<Raw>(data.ToString());

                /// quick fixes
                //raw.PRICE = NumberHelper.Rounder(raw.PRICE);
                //raw.CHANGEPCT24HOUR = NumberHelper.Rounder(raw.CHANGEPCT24HOUR);
                //raw.CHANGE24HOUR = NumberHelper.Rounder(raw.CHANGE24HOUR);
                //raw.PRICE = NumberHelper.Rounder(raw.PRICE);
                
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
            string categories = (filters.Count == 0) ? null : string.Join(",", filters);

            object resp;
            try {
                resp = await Ioc.Default.GetService<ICryptoCompare>().GetNews(categories);
                var news = JsonSerializer.Deserialize<NewsResponse>(resp.ToString());
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
            try {
                var resp = await Ioc.Default.GetService<ICryptoCompare>().GetNewsCategories();
                return JsonSerializer.Deserialize<List<NewsCategories>>(resp.ToString());
            }
            catch (Exception ex) {
                return new List<NewsCategories>() { new NewsCategories() };
            }
        }

    }
}
