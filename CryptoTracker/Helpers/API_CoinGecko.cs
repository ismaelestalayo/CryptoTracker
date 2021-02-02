using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {

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
        public double aed { get; set; }
        public double ars { get; set; }
        public double aud { get; set; }
        public double bch { get; set; }
        public double bdt { get; set; }
        public double bhd { get; set; }
        public double bmd { get; set; }
        public double bnb { get; set; }
        public double brl { get; set; }
        public double btc { get; set; }
        public double cad { get; set; }
        public double chf { get; set; }
        public double clp { get; set; }
        public double cny { get; set; }
        public double czk { get; set; }
        public double dkk { get; set; }
        public double eos { get; set; }
        public double eth { get; set; }
        public double eur { get; set; }
        public double gbp { get; set; }
        public double hkd { get; set; }
        public double huf { get; set; }
        public double idr { get; set; }
        public double ils { get; set; }
        public double inr { get; set; }
        public double jpy { get; set; }
        public double krw { get; set; }
        public double kwd { get; set; }
        public double lkr { get; set; }
        public double ltc { get; set; }
        public double mmk { get; set; }
        public double mxn { get; set; }
        public double myr { get; set; }
        public double nok { get; set; }
        public double nzd { get; set; }
        public double php { get; set; }
        public double pkr { get; set; }
        public double pln { get; set; }
        public double rub { get; set; }
        public double sar { get; set; }
        public double sek { get; set; }
        public double sgd { get; set; }
        public double thb { get; set; }
        public double twd { get; set; }
        public double uah { get; set; }
        public double usd { get; set; }
        public double vef { get; set; }
        public double vnd { get; set; }
        public double xag { get; set; }
        public double xau { get; set; }
        public double xdr { get; set; }
        public double xlm { get; set; }
        public double xrp { get; set; }
        public double zar { get; set; }
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





    // ###############################################################################################
    /// <summary>
    /// Class to interact with CoinGecko's API
    /// </summary>
    class API_CoinGecko {
        // Get current data (name, price, market...) for a coin
        public static async Task<CoinData> GetCoin(string coin) {
            if (coin == "XRP")
                coin = "Ripple";
            coin = coin.ToLower(CultureInfo.InvariantCulture);
            string URL = string.Format("https://api.coingecko.com/api/v3/coins/{0}?localization=false&tickers=false&community_data=false&developer_data=false", coin);
            Uri uri = new Uri(URL);

            
            try {
                string response = await App.Client.GetStringAsync(uri);
                var coinData = JsonSerializer.Deserialize<CoinData>(response);
                string descr = await App.GetCoinDescription(coinData.symbol.ToUpper(CultureInfo.InvariantCulture), 5).ConfigureAwait(true);
                coinData.description.en = (!String.IsNullOrEmpty(descr)) ? descr : coinData.description.en.Split('\n')[0];

                return coinData;
            }
            catch (Exception ex) {
                return new CoinData();
            }
        }
    }
}
