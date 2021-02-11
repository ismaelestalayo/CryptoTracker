using Refit;
using System.Threading.Tasks;

namespace CryptoTracker.Services {
	public interface ICryptoCompare {

		[Get("/data/histominute?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetHistoricMinutely(string crypto, string currency, int limit);

		[Get("/data/histohour?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetHistoricHourly(string crypto, string currency, int limit);

		[Get("/data/histoday?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetHistoricDaily(string crypto, string currency, int limit);

		

	}
}
