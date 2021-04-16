using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
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
        [Get("/data/price?fsym={crypto}&tsyms={currency}")]
        Task<string> GetPrice(string crypto, string currency);

        [Get("/data/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
        Task<object> GetHistoric(string time, string crypto, string currency, int limit, int aggregate = 1);

        [Get("/data/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&allData=true")]
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
        public static async Task<double> GetPrice_Extension(this ICryptoCompare service, string crypto, string currency) {
            var response = await service.GetPrice(crypto, currency);
            var data = response.ToString();
            double price = 7;
            double.TryParse(data.Split(":")[1].Replace("}", ""), out price);
            return price;
        }

        public static async Task<List<HistoricPrice>> GetHistoric_(this ICryptoCompare service,
            string crypto, string time, int limit, int aggregate = 1) {

            var currency = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettings.Currency);
            
            var NullValue = new List<HistoricPrice>() { new HistoricPrice() };

            object resp;
            try {
                if (limit == 0)
                    resp = await service.GetHistoricAll(time, crypto, currency);
                else
                    resp = await service.GetHistoric(time, crypto, currency, limit, aggregate);

                var response = JsonSerializer.Deserialize<object>(resp.ToString());

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

                return historic;
            }
            catch (Exception ex) {
                var z = ex.Message;
                return NullValue;
            }
        }

    }
}
