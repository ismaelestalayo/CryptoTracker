using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Services;

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

        private LocalSettings localSettings = new LocalSettings();

        internal static async Task<List<HistoricPrice>> GetWeeklyHistAsync(string crypto) {
            var currency = Ioc.Default.GetService<LocalSettings>().Get<string>(UserSettingsConstants.Currency);
            var NullValue = new List<HistoricPrice>() { new HistoricPrice() { Average = 1, DateTime = DateTime.Today } };

            try {
                var resp = await Ioc.Default.GetService<ICryptoCompare>().GetHistoric("hour", crypto, currency, 168);
                var response = JsonSerializer.Deserialize<object>(resp.ToString());

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
