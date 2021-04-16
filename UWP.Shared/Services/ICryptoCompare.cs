using Refit;
using System.Threading.Tasks;

namespace UWP.Services {
    public interface ICryptoCompare {

		[Get("/data/price?fsym={crypto}&tsyms={currency}")]
		Task<string> GetPrice(string crypto, string currency);

		[Get("/data/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetHistoric(string time, string crypto, string currency, int limit, int aggregate = 1);

		[Get("/data/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&allData=true")]
		Task<object> GetHistoricAll(string time, string crypto, string currency);

		[Get("/data/top/exchanges?fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetExchanges(string crypto, string currency, int limit);

		[Get("/data/top/totalvolfull?tsym={crypto}&limit={limit}")]
		Task<object> GetTop100(string crypto, int limit);

		[Get("/data/pricemultifull?fsyms={crypto}&tsyms={currency}&e=CCCAGG")]
		Task<object> GetCoinStats(string crypto, string currency);

		[Get("/data/v2/news/?lang=EN")]
		Task<object> GetNews(string categories = null);

		[Get("/data/news/categories")]
		Task<object> GetNewsCategories();
	}

	public static class CryptoCompareExtensions {
		public static async Task<double> GetPrice(this ICryptoCompare service, string crypto, string currency) {
			var response = await service.GetPrice(crypto, currency);
			var data = response.ToString();
			double price = 0;
			double.TryParse(data.Split(":")[1].Replace("}", ""), out price);
			return price;
		}
    }
}
