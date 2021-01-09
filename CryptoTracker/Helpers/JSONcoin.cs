using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CryptoTracker.Helpers {
    internal class JSONcoin {
        internal int Id { get; set; }
        internal string ImageUrl { get; set; }
        internal string Symbol { get; set; }
        internal string FullName { get; set; }

        public static List<JSONcoin> HandleJSON(JToken data) {
            var coins = new List<JSONcoin>();
            
            foreach (var coin in data["Data"]) {
				try {					
                    var c = ((JContainer)coin).First;
                    coins.Add(new JSONcoin {
                        Id = (int)c["Id"],
                        ImageUrl = c["ImageUrl"].ToString(),
                        Symbol = c["Symbol"].ToString(),
                        FullName = c["FullName"].ToString()
                    });
                } catch (NullReferenceException) { }
            }
            
			
            return coins;
        }
    }
}
