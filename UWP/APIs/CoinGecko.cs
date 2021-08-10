using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;

namespace UWP.APIs {
    class CoinGecko {

        internal async static Task<GlobalStats> GetGlobalStats() {
            try {
                var resp = await Ioc.Default.GetService<ICoinGecko>().GetGlobalStats();
                var response = JsonSerializer.Deserialize<object>(resp.ToString());
                
                var data = ((JsonElement)response).GetProperty("data");

                var stats = new GlobalStats();
                var currency = App.currency.ToLowerInvariant();
                var btcDominance = double.Parse(
                    data.GetProperty("market_cap_percentage").GetProperty("btc").ToString()
                );
                var totalVolume = double.Parse(
                    data.GetProperty("total_volume").GetProperty(currency).ToString()
                );
                var totalMarketCap = double.Parse(
                    data.GetProperty("total_market_cap").GetProperty(currency).ToString()
                );

                stats.BtcDominance = Math.Round(btcDominance, 2);
                stats.TotalVolume = NumberHelper.AddUnitPrefix(totalVolume);
                stats.TotalMarketCap = NumberHelper.AddUnitPrefix(totalMarketCap);
                stats.CurrencySymbol = App.currencySymbol;
                return stats;

            }
            catch (Exception ex) {
                return new GlobalStats();
            }
        }

        public static async Task<CoinData> GetCoin(string coin) {
            if (coin == "XRP")
                coin = "Ripple";
            coin = coin.ToLower(CultureInfo.InvariantCulture);

            try {
                var resp = await Ioc.Default.GetService<ICoinGecko>().GetCoin(coin);
                var coinData = JsonSerializer.Deserialize<CoinData>(resp.ToString());
                string descr = await App.GetCoinDescription(coinData.symbol.ToUpper(CultureInfo.InvariantCulture), 5).ConfigureAwait(true);
                coinData.description.en = (!String.IsNullOrEmpty(descr)) ? descr : coinData.description.en.Split('\n')[0];

                return coinData;
            }
            catch (Exception ex) {
                return new CoinData();
            }
        }




        public class Description {
            public string en { get; set; } = "No description.\n";
        }

        public class Links {
            public List<string> homepage { get; set; }
            public List<string> blockchain_site { get; set; }
            public string twitter_screen_name { get; set; }
            public string subreddit_url { get; set; }
        }

        public class Image {
            public string thumb { get; set; }
            public string small { get; set; }
            public string large { get; set; }
        }

        public class CurrencyDoubleArray {
            public double aed { get; set; } = 0;
            public double ars { get; set; } = 0;
            public double aud { get; set; } = 0;
            public double bch { get; set; } = 0;
            public double bdt { get; set; } = 0;
            public double bhd { get; set; } = 0;
            public double bmd { get; set; } = 0;
            public double bnb { get; set; } = 0;
            public double brl { get; set; } = 0;
            public double btc { get; set; } = 0;
            public double cad { get; set; } = 0;
            public double chf { get; set; } = 0;
            public double clp { get; set; } = 0;
            public double cny { get; set; } = 0;
            public double czk { get; set; } = 0;
            public double dkk { get; set; } = 0;
            public double eos { get; set; } = 0;
            public double eth { get; set; } = 0;
            public double eur { get; set; } = 0;
            public double gbp { get; set; } = 0;
            public double hkd { get; set; } = 0;
            public double huf { get; set; } = 0;
            public double idr { get; set; } = 0;
            public double ils { get; set; } = 0;
            public double inr { get; set; } = 0;
            public double jpy { get; set; } = 0;
            public double krw { get; set; } = 0;
            public double kwd { get; set; } = 0;
            public double lkr { get; set; } = 0;
            public double ltc { get; set; } = 0;
            public double mmk { get; set; } = 0;
            public double mxn { get; set; } = 0;
            public double myr { get; set; } = 0;
            public double nok { get; set; } = 0;
            public double nzd { get; set; } = 0;
            public double php { get; set; } = 0;
            public double pkr { get; set; } = 0;
            public double pln { get; set; } = 0;
            public double rub { get; set; } = 0;
            public double sar { get; set; } = 0;
            public double sek { get; set; } = 0;
            public double sgd { get; set; } = 0;
            public double thb { get; set; } = 0;
            public double twd { get; set; } = 0;
            public double uah { get; set; } = 0;
            public double usd { get; set; } = 0;
            public double vef { get; set; } = 0;
            public double vnd { get; set; } = 0;
            public double xag { get; set; } = 0;
            public double xau { get; set; } = 0;
            public double xdr { get; set; } = 0;
            public double xlm { get; set; } = 0;
            public double xrp { get; set; } = 0;
            public double zar { get; set; } = 0;
        }

        public class CurrencyDateArray {
            public DateTime aed { get; set; }
            public DateTime ars { get; set; }
            public DateTime aud { get; set; }
            public DateTime bch { get; set; }
            public DateTime bdt { get; set; }
            public DateTime bhd { get; set; }
            public DateTime bmd { get; set; }
            public DateTime bnb { get; set; }
            public DateTime brl { get; set; }
            public DateTime btc { get; set; }
            public DateTime cad { get; set; }
            public DateTime chf { get; set; }
            public DateTime clp { get; set; }
            public DateTime cny { get; set; }
            public DateTime czk { get; set; }
            public DateTime dkk { get; set; }
            public DateTime eos { get; set; }
            public DateTime eth { get; set; }
            public DateTime eur { get; set; }
            public DateTime gbp { get; set; }
            public DateTime hkd { get; set; }
            public DateTime huf { get; set; }
            public DateTime idr { get; set; }
            public DateTime ils { get; set; }
            public DateTime inr { get; set; }
            public DateTime jpy { get; set; }
            public DateTime krw { get; set; }
            public DateTime kwd { get; set; }
            public DateTime lkr { get; set; }
            public DateTime ltc { get; set; }
            public DateTime mmk { get; set; }
            public DateTime mxn { get; set; }
            public DateTime myr { get; set; }
            public DateTime nok { get; set; }
            public DateTime nzd { get; set; }
            public DateTime php { get; set; }
            public DateTime pkr { get; set; }
            public DateTime pln { get; set; }
            public DateTime rub { get; set; }
            public DateTime sar { get; set; }
            public DateTime sek { get; set; }
            public DateTime sgd { get; set; }
            public DateTime thb { get; set; }
            public DateTime twd { get; set; }
            public DateTime uah { get; set; }
            public DateTime usd { get; set; }
            public DateTime vef { get; set; }
            public DateTime vnd { get; set; }
            public DateTime xag { get; set; }
            public DateTime xau { get; set; }
            public DateTime xdr { get; set; }
            public DateTime xlm { get; set; }
            public DateTime xrp { get; set; }
            public DateTime zar { get; set; }
        }

        public class MarketData {
            public CurrencyDoubleArray current_price { get; set; }
            public CurrencyDoubleArray ath { get; set; }
            public CurrencyDoubleArray ath_change_percentage { get; set; }
            public CurrencyDateArray ath_date { get; set; }
            public CurrencyDoubleArray atl { get; set; }
            public CurrencyDoubleArray atl_change_percentage { get; set; }
            public CurrencyDateArray atl_date { get; set; }
            public CurrencyDoubleArray market_cap { get; set; }
            public int market_cap_rank { get; set; }
            public CurrencyDoubleArray total_volume { get; set; }
            public CurrencyDoubleArray high_24h { get; set; }
            public CurrencyDoubleArray low_24h { get; set; }
            public double price_change_24h { get; set; }
            public double price_change_percentage_24h { get; set; }
            public double price_change_percentage_7d { get; set; }
            public double price_change_percentage_14d { get; set; }
            public double price_change_percentage_30d { get; set; }
            public double price_change_percentage_60d { get; set; }
            public double price_change_percentage_200d { get; set; }
            public double price_change_percentage_1y { get; set; }
            public double market_cap_change_24h { get; set; }
            public double market_cap_change_percentage_24h { get; set; }
            public CurrencyDoubleArray price_change_24h_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_1h_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_24h_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_7d_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_14d_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_30d_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_60d_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_200d_in_currency { get; set; }
            public CurrencyDoubleArray price_change_percentage_1y_in_currency { get; set; }
            public CurrencyDoubleArray market_cap_change_24h_in_currency { get; set; }
            public CurrencyDoubleArray market_cap_change_percentage_24h_in_currency { get; set; }
            public object total_supply { get; set; }
            public double circulating_supply { get; set; }
            public DateTime last_updated { get; set; }
        }

        public class CoinData : INotifyPropertyChanged {
            public string id { get; set; } = "null";
            public string symbol { get; set; } = "NULL";
            public string name { get; set; } = "Null";
            public string hashing_algorithm { get; set; } = "null";
            public Description description { get; set; } = new Description();
            public Links links { get; set; }
            public Image image { get; set; }
            public string genesis_date { get; set; } = "null";
            public int market_cap_rank { get; set; } = 0;
            public MarketData market_data { get; set; }
            public DateTime last_updated { get; set; } = DateTime.Now;

            public event PropertyChangedEventHandler PropertyChanged;
            public void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
