using System.Collections.Generic;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Models;
using UWP.Services;
using UWP.Shared.Constants;

namespace UWP.Background {
    public sealed class AlertsManager {
        private static LocalSettings localSettings = new LocalSettings();
        private static string currency = localSettings.Get<string>(UserSettings.Currency);
        private static string currencySym = Currencies.GetCurrencySymbol(currency);


        

        public static void ChangeAlertState() {

        }

        public static void DeleteAlert() {

        }
    }
}
