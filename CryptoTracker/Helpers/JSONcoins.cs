using Newtonsoft.Json.Linq;

namespace CryptoTracker.Helpers {
    internal class JSONcoins {
        internal int Id { get; set; }
        internal string Name { get; set; }
        internal string FullName { get; set; }

        public static void HandleJSON(JToken data) {

            int lastIndex = ((JContainer)data["Data"]).Count;
            for (int i = 0; i < lastIndex; i++) {
                App.coinList.Add(new JSONcoins {
                    Id       = (int)data["Data"][i]["CoinInfo"]["Id"],
                    Name     = data["Data"][i]["CoinInfo"]["Name"].ToString(),
                    FullName = data["Data"][i]["CoinInfo"]["FullName"].ToString()
                });
            }

            (App.coinList).Sort((x, y) => x.Name.CompareTo(y.Name) );
        }
    }
}
