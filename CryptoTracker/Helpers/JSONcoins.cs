using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CryptoTracker.Helpers {
    internal class JSONcoins {
        internal int Id { get; set; }
        internal string Name { get; set; }
        internal string FullName { get; set; }

        public static List<JSONcoins> HandleJSON(JToken data) {
            var coins = new List<JSONcoins>();

            int lastIndex = ((JContainer)data["Data"]).Count;
            for (int i = 0; i < lastIndex; i++) {
                coins.Add(new JSONcoins {
                    Id       = (int)data["Data"][i]["CoinInfo"]["Id"],
                    Name     = data["Data"][i]["CoinInfo"]["Name"].ToString(),
                    FullName = data["Data"][i]["CoinInfo"]["FullName"].ToString()
                });
            }

            return coins;
        }
    }
}
