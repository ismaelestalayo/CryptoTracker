using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;

namespace UWP.Shared.Helpers {
    public class PortfolioHelper {

        public async static Task<List<PurchaseModel>> GetPortfolio(string filterCrypto = "") {
            var portfolio = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio);

            if (filterCrypto != "")
                portfolio = portfolio.Where(p => p.Crypto == filterCrypto).ToList();

            return (portfolio.Count == 0) ? new List<PurchaseModel>() : portfolio;
        }
    }
}
