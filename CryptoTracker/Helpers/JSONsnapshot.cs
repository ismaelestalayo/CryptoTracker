using Newtonsoft.Json.Linq;
using System;

namespace CryptoTracker.Helpers {

    internal class JSONsnapshot {        
        internal string H1Text { get; set; }
        internal string Description { get; set; }
        internal string Features { get; set; }
        internal string Technology { get; set; }
        internal string Algorithm { get; set; }
        internal string StartDate { get; set; }
        internal string Twitter { get; set; }
        internal string WebSiteURL { get; set; }
        internal long TotalCoinSupply { get; set; }

        public static JSONsnapshot HandleJSON(JToken data) {

            try {
                JSONsnapshot z = new JSONsnapshot {
                    H1Text      = data["Data"]["General"]["H1Text"].ToString(),
                    Description = data["Data"]["General"]["Description"].ToString(),
                    Features    = data["Data"]["General"]["Features"].ToString(),
                    Technology  = data["Data"]["General"]["Technology"].ToString(),
                    Algorithm   = data["Data"]["General"]["Algorithm"].ToString(),
                    StartDate   = data["Data"]["General"]["StartDate"].ToString(),
                    Twitter     = data["Data"]["General"]["Twitter"].ToString(),
                    WebSiteURL  = data["Data"]["General"]["WebsiteUrl"].ToString(),
                    TotalCoinSupply = long.Parse(data["Data"]["General"]["TotalCoinSupply"].ToString())
                };
                return z;
            } catch (Exception ex) {
                JSONsnapshot z = new JSONsnapshot {
                    H1Text = "Error"
                };
                return z;
            }
        }
    }
}