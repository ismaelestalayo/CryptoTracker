using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {
    class JSONstats {
        internal string Low24       { get; set; }
        internal string High24      { get; set; }
        internal string Open24      { get; set; }
        internal string Volume24    { get; set; }
        internal string Volume24To  { get; set; }
        internal string Change24    { get; set; }
        internal string Change24pct { get; set; }
        internal string Supply      { get; set; }
        internal string Marketcap   { get; set; }

        public static JSONstats HandleStatsJSON(JToken data, string crypto, string coin) {
            //if (data["Response"] == null)
            //    return HandleNULL();

            JSONstats a = new JSONstats();

            try {
                string fromSymbol = data["DISPLAY"][crypto][coin]["FROMSYMBOL"].ToString();
                string toSymbol = data["DISPLAY"][crypto][coin]["TOSYMBOL"].ToString();

                a.Open24 = ((double)data["RAW"][crypto][coin]["OPEN24HOUR"]).ToString("N2") + toSymbol;
                a.High24 = ((double)data["RAW"][crypto][coin]["HIGH24HOUR"]).ToString("N2") + toSymbol;
                a.Low24 = ((double)data["RAW"][crypto][coin]["LOW24HOUR"]).ToString("N2") + toSymbol;
                a.Change24 = ((double)data["RAW"][crypto][coin]["CHANGE24HOUR"]).ToString("N2") + toSymbol;
                a.Change24pct = ((double)data["RAW"][crypto][coin]["CHANGEPCT24HOUR"]).ToString() + "%";
                a.Supply = ((double)data["RAW"][crypto][coin]["SUPPLY"]).ToString("N0") + fromSymbol;
                a.Marketcap = ((double)data["RAW"][crypto][coin]["MKTCAP"]).ToString("N2") + toSymbol;
                a.Volume24 = ((double)data["RAW"][crypto][coin]["TOTALVOLUME24H"]).ToString("N2") + fromSymbol;
                a.Volume24To = ((double)data["RAW"][crypto][coin]["TOTALVOLUME24HTO"]).ToString("N2") + toSymbol;

                return a;
            } catch(Exception) {

                string fromSymbol = "null";
                string toSymbol = "null";

                a.Open24 = toSymbol;
                a.High24 = toSymbol;
                a.Low24 = toSymbol;
                a.Change24 = toSymbol;
                a.Change24pct = 0 + "%";
                a.Supply = fromSymbol;
                a.Marketcap = toSymbol;
                a.Volume24 = fromSymbol;
                a.Volume24To = toSymbol;

                return a;
            }
            
        }
    }

}
