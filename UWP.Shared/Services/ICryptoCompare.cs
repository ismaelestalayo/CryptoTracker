using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Shared.Models;

namespace UWP.Services {
    public interface ICryptoCompare {

        /* ###############################################################################################
         * Gets the current price of a coin
         * 
         * Arguments: 
         * - crypto: BTC ETH...
         * - market: CCCAGG Bitstamp Bitfinex Coinbase HitBTC Kraken Poloniex
        */
        [Get("/data/all/coinlist?summary=true")]
        Task<string> GetAllCoins();

        [Get("/data/price?fsym={crypto}&tsyms={currency}")]
        Task<string> GetPrice(string crypto, string currency);

        [Get("/data/v2/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
        Task<object> GetHistoric(string time, string crypto, string currency, int limit, int aggregate = 1);

        [Get("/data/v2/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&allData=true")]
        Task<object> GetHistoricAll(string time, string crypto, string currency);

        [Get("/data/top/exchanges?fsym={crypto}&tsym={currency}&limit={limit}")]
        Task<object> GetExchanges(string crypto, string currency, int limit);

        [Get("/data/top/totalvolfull?tsym={crypto}&limit={limit}")]
        Task<object> GetTop100(string crypto, int limit);

        [Get("/data/pricemultifull?fsyms={crypto}&tsyms={currency}&e=CCCAGG")]
        Task<object> GetCoinStats(string crypto, string currency);

        [Get("/data/v2/news/?lang=EN")]
        Task<object> GetNews(string categories = null);

        [Get("/data/news/categories")]
        Task<object> GetNewsCategories();
    }

    public static class CryptoCompareExtensions {
        public static async Task<List<CoinCryptoCompare>> GetAllCoins_(this ICryptoCompare service) {
            try {
                var resp = await service.GetAllCoins();
                var response = JsonSerializer.Deserialize<object>(resp.ToString());
                var okey = ((JsonElement)response).GetProperty("Response").ToString();
                if (okey != "Success")
                    throw new Exception();

                var data = ((JsonElement)response).GetProperty("Data");
                var d = JsonSerializer.Deserialize<Dictionary<string, CoinCryptoCompare>>(data.ToString());

                return d.Values.ToList();
            }
            catch (Exception ex) {
                return new List<CoinCryptoCompare>();
            }
        }
        public static async Task<double> GetPrice_Extension(this ICryptoCompare service, string crypto, string currency) {
            double price = 0;
            try {
                var response = await service.GetPrice(crypto, currency);
                var data = response.ToString();
                double.TryParse(data.Split(":")[1].Replace("}", ""), out price);
                return price;
            }
            catch (Exception ex) {
                return price;
            }
        }

        public static async Task<List<HistoricPrice>> GetHistoric_(this ICryptoCompare service,
            string crypto, string time, int limit, int aggregate = 1) {

            var currency = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.Currency);
            

            object resp;
            try {
                if (limit == 0)
                    resp = await service.GetHistoricAll(time, crypto, currency);
                else
                    resp = await service.GetHistoric(time, crypto, currency, limit, aggregate);

                var response = JsonSerializer.Deserialize<object>(resp.ToString());

                var okey = ((JsonElement)response).GetProperty("Response").ToString();
                var data = ((JsonElement)response).GetProperty("Data");

                var timeTo = data.GetProperty("TimeTo").ToString();
                var timeFrom = data.GetProperty("TimeFrom").ToString();
                if (!okey.Equals("Success", StringComparison.InvariantCultureIgnoreCase) || timeTo == timeFrom)
                    throw new Exception();

                var historic = JsonSerializer.Deserialize<List<HistoricPrice>>(data.GetProperty("Data").ToString());

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

                return historic;
            }
            catch (Exception ex) {
                var z = ex.Message;
                return new List<HistoricPrice>();
            }
        }

    }

    public class CoinCryptoCompare {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string FullName { get; set; }
    }
}
