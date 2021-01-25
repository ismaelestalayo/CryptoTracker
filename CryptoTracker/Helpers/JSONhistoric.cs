using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

namespace CryptoTracker.Helpers {

    internal class JSONhistoric {
        internal int LinuxTime;
        internal string Date { get; set; }
        internal DateTime DateTime { get; set; }
        internal float Low { get; set; } = 0;
        internal float High { get; set; } = 0;
        internal float Open { get; set; } = 0;
        internal float Close { get; set; } = 0;
        internal float Volumefrom { get; set; } = 0;
        internal float Volumeto { get; set; } = 0;

        public static List<JSONhistoric> HandleHistoricJSON(JToken data) {

            if (data["Response"].ToString().Equals("Error")) 
                throw new NullReferenceException("Manually caught null coin exception.");
            
            var hist = new List<JSONhistoric>();

            int lastIndex = ((JContainer)data["Data"]).Count;
            foreach(JToken d in data["Data"]) {
                hist.Add(GetHistoricPoint(d));
            }
            return hist;
        }

        public static List<JSONhistoric> HandleHistoricJSONnull(int limit) {

            var hist = new List<JSONhistoric>();
            for (int i = 0; i < limit; i++) {
                hist.Add( new JSONhistoric() );
            }
            return hist;
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
