using Refit;
using System.Threading.Tasks;

namespace UWP.Services {
    public interface ICoinGecko {

		[Get("/global")]
		Task<string> GetGlobalStats();

		[Get("/coins/{coin}?localization=false&tickers=false&community_data=false&developer_data=false")]
		Task<string> GetCoin(string coin);

	}
}
