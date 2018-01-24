using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {

    class JSONexchanges {
        // Must be public to work ¿because they are in another folder?
        public String Exchange { get; set; }
        public double Price { get; set; }
        public String FSym { get; set; }
        public String TSym { get; set; }
        public String TSymbol { get; set; }
        public String T_Volume24 { get; set; }
        public String T_Volume24To { get; set; }

        public static JSONexchanges GetExchanges(JToken data) {
            JSONexchanges p = new JSONexchanges();

            p.Exchange     = data["exchange"].ToString();
            p.FSym         = data["fromSymbol"].ToString();           //ETH
            p.TSym         = data["toSymbol"].ToString();             //EUR
            p.TSymbol      = CurrencyHelper.CurrencyToSymbol(p.TSym); //€
            p.T_Volume24   = "Vol. 24h: " + (Math.Round((double)data["volume24h"], 2)).ToString() + p.FSym;
            p.T_Volume24To = "Vol. To 24h: " + (Math.Round((double)data["volume24hTo"], 2)).ToString() + App.coinSymbol;

            p.Price = App.GetPrice(p.FSym, p.TSym, p.Exchange);
            p.Price = Math.Round(p.Price, 4);

            return p;
        }

    }
}
