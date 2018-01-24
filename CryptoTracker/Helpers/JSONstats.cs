using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {
    class JSONstats {
        internal float Low24 { get; set; }
        internal float High24 { get; set; }
        internal float Open24 { get; set; }
        internal float Volume24 { get; set; }
        internal float Volume24To { get; set; }

        public static JSONstats HandleStatsJSON(JToken data) {
            JSONstats a = new JSONstats();

            a.Low24      = (float)Math.Round((float)data["LOW24HOUR"],      2);
            a.High24     = (float)Math.Round((float)data["HIGH24HOUR"],     2);
            a.Open24     = (float)Math.Round((float)data["OPEN24HOUR"],     2);
            a.Volume24   = (float)Math.Round((float)data["VOLUME24HOUR"],   2);
            a.Volume24To = (float)Math.Round((float)data["VOLUME24HOURTO"], 2);

            return a;
        }
    }
}
