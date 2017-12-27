using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {
    internal class PricePoint {
        public int LinuxTime;
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
        public float Low { get; set; }
        public float High { get; set; }
        public float Open { get; set; }
        public float Close { get; set; }
        public float Volumefrom { get; set; }
        public float Volumeto { get; set; }

        // For stats only
        public float Low24 { get; set; }
        public float High24 { get; set; }
        public float Open24 { get; set; }
        public float Volume24 { get; set; }
        public float Volume24To { get; set; }

        public static PricePoint GetPricePointHisto(JToken data) {
            PricePoint p = new PricePoint();

            p.LinuxTime  = (int)data["time"];
            p.Low        = (float)Math.Round((float)data["low"], 2);
            p.High       = (float)Math.Round((float)data["high"], 2);
            p.Open       = (float)Math.Round((float)data["open"], 2);
            p.Close      = (float)Math.Round((float)data["close"], 2);
            p.Volumefrom = (float)Math.Round((float)data["volumefrom"], 2);
            p.Volumeto   = (float)Math.Round((float)data["volumeto"], 2);

            int unixTimeStamp = p.LinuxTime;

            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            p.DateTime = date;
            p.Date = date.ToString();

            return p;
        }

        public static PricePoint GetPricePointStats(JToken data) {
            PricePoint p = new PricePoint();

            p.Low24      = (float)Math.Round((float)data["LOW24HOUR"], 2);
            p.High24     = (float)Math.Round((float)data["HIGH24HOUR"], 2);
            p.Open24     = (float)Math.Round((float)data["OPEN24HOUR"], 2);
            p.Volume24   = (float)Math.Round((float)data["VOLUME24HOUR"], 2);
            p.Volume24To = (float)Math.Round((float)data["VOLUME24HOURTO"], 2);

            return p;
        }
    }
}
