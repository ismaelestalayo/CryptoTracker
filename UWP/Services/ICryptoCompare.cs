using Refit;
using System.Threading.Tasks;

namespace UWP.Services {
	public interface ICryptoCompare {

		[Get("/data/histo{time}?e=CCCAGG&fsym={crypto}&tsym={currency}&limit={limit}")]
		Task<object> GetHistoric(string time, string crypto, string currency, int limit, int aggregate = 1);


	}
}
