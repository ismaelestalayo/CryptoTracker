using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace UWP.Background {

    public sealed class HistoricPrice {
        public int time { get; set; }
        public float high { get; set; } = 0;
        public float low { get; set; } = 0;
        public float open { get; set; } = 0;
        public float close { get; set; } = 0;
        public float volumefrom { get; set; } = 0;
        public float volumeto { get; set; } = 0;

        internal float Average { get; set; } = 0;
        internal string Date { get; set; }
        internal DateTime DateTime { get; set; }
    }

    class GetHistoDupe {

        internal static HttpClient Client = new HttpClient();

        internal static async Task<List<HistoricPrice>> GetWeeklyHistAsync(string crypto) {
            var currency = "EUR";
            
            var NullValue = new List<HistoricPrice>() { new HistoricPrice() { Average = 1, DateTime = DateTime.Today } };

            string URL = string.Format("https://min-api.cryptocompare.com/data/histohour?e=CCCAGG&fsym={0}&tsym={1}&limit={2}",
                crypto, currency, 168);


            try {
                var responseString = await Client.GetStringAsync(new Uri(URL));
                var response = JsonSerializer.Deserialize<object>(responseString);

                var okey = ((JsonElement)response).GetProperty("Response").ToString();

                if (okey != "Success")
                    return NullValue;

                var data = ((JsonElement)response).GetProperty("Data").ToString();
                var historic = JsonSerializer.Deserialize<List<HistoricPrice>>(data);

                // Add calculation of dates and average values
                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                foreach (var h in historic) {
                    h.Average = (h.high + h.low) / 2;
                    DateTime d = dtDateTime.AddSeconds(h.time).ToLocalTime();
                    h.DateTime = d;
                    h.Date = d.ToString();
                }

                return historic;
            }
            catch (Exception ex) {
                return NullValue;
            }
        }
    }
}
