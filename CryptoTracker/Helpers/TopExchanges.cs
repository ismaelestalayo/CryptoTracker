using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {

    class TopExchanges {
        // Must be public to work ¿because they are in another folder?
        public String Exchange { get; set; }
        public double Price { get; set; }
        public String FSym { get; set; }
        public String TSym { get; set; }
        public String T_Volume24 { get; set; }
        public String T_Volume24To { get; set; }

        public static TopExchanges GetExchanges(JToken data) {
            TopExchanges p = new TopExchanges();

            p.Exchange     = data["exchange"].ToString();
            p.FSym         = data["fromSymbol"].ToString();
            p.TSym         = data["toSymbol"].ToString();
            p.T_Volume24   = "Vol. 24h: " + (Math.Round((double)data["volume24h"], 2)).ToString();
            p.T_Volume24To = "Vol. To 24h: " + (Math.Round((double)data["volume24hTo"], 2)).ToString();

            p.Price = App.GetPrice(p.FSym, p.TSym, p.Exchange);

            return p;
        }

    }
}
