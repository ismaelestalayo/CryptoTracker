using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {

    internal class JSONhistoric {
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

            if (data["Response"].ToString().Equals("Error")) 
                throw new NullReferenceException("Manually caught null coin exception.");
            

            App.historic.Clear();

            int lastIndex = ((JContainer)data["Data"]).Count;
            for (int i = 0; i < lastIndex; i++) {
                App.historic.Add(GetHistoricPoint(data["Data"][i]));
            }
        }

        public static void HandleHistoricJSONnull(string crypto, int limit) {
            App.historic.Clear();
            
            for (int i = 0; i < limit; i++) {
                App.historic.Add( new JSONhistoric() {
                    Low         = 0,
                    High        = 0,
                    Open        = 0,
                    Close       = 0,
                    Volumefrom  = 0,
                    Volumeto    = 0
                } );
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        private static JSONhistoric GetHistoricPoint(JToken data) {
            JSONhistoric a = new JSONhistoric();

            a.LinuxTime = (int)data["time"];
            a.Low = (float)Math.Round((float)data["low"], 3);
            a.High = (float)Math.Round((float)data["high"], 3);
            a.Open = (float)Math.Round((float)data["open"], 3);
            a.Close = (float)Math.Round((float)data["close"], 3);
            a.Volumefrom = (float)Math.Round((float)data["volumefrom"], 3);
            a.Volumeto = (float)Math.Round((float)data["volumeto"], 3);

            int unixTimeStamp = a.LinuxTime;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            a.DateTime = date;
            a.Date = date.ToString();

            return a;
        }
    }
}
