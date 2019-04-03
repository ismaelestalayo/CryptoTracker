using Newtonsoft.Json.Linq;

namespace CryptoTracker.Helpers {

    class JSONexchanges {
        // Must be public to work ¿because they are in another folder?
        public string Exchange { get; set; }
        public string Price { get; set; }
        public string FSym { get; set; }
        public string FSymbol { get; set; }
        public string TSym { get; set; }
        public string TSymbol { get; set; }
        public string T_Volume24 { get; set; }
        public string T_Volume24To { get; set; }

        public static JSONexchanges GetExchanges(JToken data) {
            JSONexchanges p = new JSONexchanges();

            p.Exchange     = data["exchange"].ToString();
            p.FSym         = data["fromSymbol"].ToString();
            p.FSymbol      = CurrencyHelper.CurrencyToSymbol(p.FSym);
            if (p.FSymbol == "ERR")
                p.FSymbol = p.FSym;
            p.TSym         = data["toSymbol"].ToString();
            p.TSymbol      = CurrencyHelper.CurrencyToSymbol(p.TSym);
            p.T_Volume24   = "Vol. 24h: " + ((double)data["volume24h"]).ToString("N0") + p.FSymbol;
            p.T_Volume24To = "Vol. To 24h: " + ((double)data["volume24hTo"]).ToString("N0") + App.coinSymbol;

            p.Price = App.GetCurrentPrice(p.FSym, p.Exchange).ToString("N2") + App.coinSymbol;

            return p;
        }

    }
}
