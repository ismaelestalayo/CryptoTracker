using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;
using Windows.UI.Xaml.Media;

namespace UWP.Shared.Helpers {
    public class PortfolioHelper {

        public async static Task<List<PurchaseModel>> GetPortfolio(string filterCrypto = "") {
            var portfolio = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio);

            if (filterCrypto != "")
                portfolio = portfolio.Where(p => p.Crypto == filterCrypto).ToList();

            return (portfolio.Count == 0) ? new List<PurchaseModel>() : portfolio;
        }

        public async static Task<PurchaseModel> UpdatePurchase(PurchaseModel purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = await CryptoCompareExtensions.GetPrice(
                    Ioc.Default.GetService<ICryptoCompare>(), purchase.Crypto, purchase.Currency);

            var curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            /// If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty >= 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                double earningz = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                purchase.Arrow = earningz < 0 ? "▼" : "▲";
                purchase.BoughtAt = priceBought;
                purchase.Delta = Math.Round(curr / priceBought, 2) * 100;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(Math.Abs(earningz), 2);
                purchase.ProfitFG = (earningz < 0) ?
                    ColorConstants.GetBrush("pastel_red") :
                    ColorConstants.GetBrush("pastel_green");
            }
            if (purchase.InvestedQty == 0)
                purchase.Delta = 0;

            return purchase;
        }
    }
}
