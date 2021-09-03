using Microsoft.Toolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;

namespace UWP.Shared.Helpers {
    public class PortfolioHelper {

        public async static Task<List<PurchaseModel>> GetPortfolio(string filterCrypto = "") {
            var portfolio = await LocalStorageHelper.ReadObject<List<PurchaseModel>>(UserStorage.Portfolio6);

            if (filterCrypto != "")
                portfolio = portfolio.Where(p => p.Crypto == filterCrypto).ToList();

            return (portfolio.Count == 0) ? new List<PurchaseModel>() : portfolio;
        }

        public async static Task AddPurchase(PurchaseModel purchase) {
            var portfolio = await GetPortfolio();
            portfolio.Add(purchase);
            await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, portfolio);
        }

        public static async Task SavePortfolio(object portfolio) {
            var type = portfolio.GetType();
            if (type == typeof(List<PurchaseModel>))
                await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, portfolio);
            else if (type == typeof(ObservableCollection<PurchaseModel>)) {
                var p = new List<PurchaseModel>((ObservableCollection<PurchaseModel>)portfolio);
                await LocalStorageHelper.SaveObject(UserStorage.Portfolio6, p);
            }
        }

        /// <summary>
        /// Calculate a purchase's worth, delta, boughtAt and profit
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        public async static Task<PurchaseModel> UpdatePurchase(PurchaseModel purchase) {
            string crypto = purchase.Crypto;

            if (purchase.Current <= 0 || (DateTime.Now - purchase.LastUpdate).TotalSeconds > 20)
                purchase.Current = await Ioc.Default.GetService<ICryptoCompare>().GetPrice_Extension(
                    purchase.Crypto, purchase.Currency);

            var curr = purchase.Current;
            purchase.Worth = Math.Round(curr * purchase.CryptoQty, 2);

            /// If the user has also filled the invested quantity, we can calculate everything else
            if (purchase.InvestedQty >= 0) {
                double priceBought = (1 / purchase.CryptoQty) * purchase.InvestedQty;
                priceBought = Math.Round(priceBought, 4);

                var diff = Math.Round((curr - priceBought) * purchase.CryptoQty, 4);
                
                purchase.Delta = Math.Round(100 * diff / purchase.InvestedQty, 2);
                purchase.BoughtAt = priceBought;
                if (purchase.Delta > 100)
                    purchase.Delta -= 100;
                purchase.Profit = Math.Round(diff, 2);
                purchase.ProfitFG = (diff < 0) ?
                    ColorConstants.GetColorBrush("pastel_red") :
                    ColorConstants.GetColorBrush("pastel_green");
            }
            if (purchase.InvestedQty == 0)
                purchase.Delta = 0;

            return purchase;
        }

        public async static Task<ObservableCollection<PurchaseModel>> GroupPortfolio(
            ObservableCollection<PurchaseModel> Purchases) {
            var query = from item in Purchases
                        group item by item.Crypto into g
                        select new { GroupName = g.Key, Items = g };
            List<PurchaseModel> grouped = new List<PurchaseModel>();
            foreach (var q in query) {
                var g = new PurchaseModel();
                var first = q.Items.First();
                g.Crypto = q.GroupName;
                g.CryptoLogo = first.CryptoLogo;
                g.CryptoName = first.CryptoName;
                g.CryptoQty = q.Items.Sum(x => x.CryptoQty);
                g.Currency = first.Currency;
                g.CurrencySymbol = first.CurrencySymbol;
                g.Current = first.Current;
                g.InvestedQty = q.Items.Sum(x => x.InvestedQty);
                g = await UpdatePurchase(g);
                grouped.Add(g);
            }
            return new ObservableCollection<PurchaseModel>(grouped);
        }
    }
}
