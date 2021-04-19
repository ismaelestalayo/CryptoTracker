using Refit;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using UWP.Helpers;

namespace UWP.Services {
    public interface ICoinPaprika {

		[Get("/v1/tickers?quotes={quote}")]
		Task<string> GetTickers(string quote);


	}

    public static class CoinPaprikaExtensions {
        
    }
}
