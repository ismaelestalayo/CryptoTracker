using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UWP.Services {
    public interface IGithub {

		[Get("/CoinList.json")]
		Task<List<CoinBasicInfo>> GetAllCoins();

	}

    public class CoinBasicInfo {
        public int id { get; set; } = 0;
        public string name { get; set; } = "NULL";
        public string symbol { get; set; } = "NULL";
        public int rank { get; set; } = 0;
    }
}
