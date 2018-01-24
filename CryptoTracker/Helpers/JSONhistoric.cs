using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {

    internal class JSONhistoric{
        internal int LinuxTime;
        internal string Date { get; set; }
        internal DateTime DateTime { get; set; }
        internal float Low { get; set; }
        internal float High { get; set; }
        internal float Open { get; set; }
        internal float Close { get; set; }
        internal float Volumefrom { get; set; }
        internal float Volumeto { get; set; }

        public static void HandleHistoricJSON(JToken data, string crypto) {
            App.historic.Clear();

            int lastIndex = ((JContainer)data["Data"]).Count;
            for (int i = 0; i < lastIndex; i++) {
                App.historic.Add( GetHistoricPoint(data["Data"][i]) );
            }

            switch (crypto) {
                case "BTC":
                    App.BTC_now= (float)Math.Round((float)data["Data"][lastIndex - 1]["close"], 2);
                    App.BTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                    break;

                case "ETH":
                    App.ETH_now = (float)Math.Round((float)data["Data"][lastIndex - 1]["close"], 2);
                    App.ETH_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                    break;

                case "LTC":
                    App.LTC_now = (float)Math.Round((float)data["Data"][lastIndex - 1]["close"], 2);
                    App.LTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                    break;

                case "XRP":
                    App.XRP_now = (float)Math.Round((float)data["Data"][lastIndex - 1]["close"], 2);
                    App.XRP_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        private static JSONhistoric GetHistoricPoint(JToken data) {
            JSONhistoric a = new JSONhistoric();

            a.LinuxTime = (int)data["time"];
            a.Low = (float)Math.Round((float)data["low"], 2);
            a.High = (float)Math.Round((float)data["high"], 2);
            a.Open = (float)Math.Round((float)data["open"], 2);
            a.Close = (float)Math.Round((float)data["close"], 2);
            a.Volumefrom = (float)Math.Round((float)data["volumefrom"], 2);
            a.Volumeto = (float)Math.Round((float)data["volumeto"], 2);

            int unixTimeStamp = a.LinuxTime;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            a.DateTime = date;
            a.Date = date.ToString();

            return a;
        }
    }
}
