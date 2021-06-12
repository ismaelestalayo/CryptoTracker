using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;

namespace UWP.Shared.Helpers {
    public class AlertsHelper {
        public async static Task<ObservableCollection<Alert>> GetAlerts() {
            var alerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            return new ObservableCollection<Alert>(alerts);
        }

        public async static Task<ObservableCollection<Alert>> GetCryptoAlerts(string crypto) {
            var allAlerts = await GetAlerts();
            var alerts = allAlerts.Where(x => x.Crypto == crypto).ToList();
            return new ObservableCollection<Alert>(alerts);
        }

        public async static void UpdateOneCryptoAlerts(string crypto, ObservableCollection<Alert> alerts) {
            var localAlerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            foreach (var alert in alerts) {
                var a = localAlerts.Select(x => x.Id == alert.Id).ToList();
            }

            //LocalStorageHelper.SaveObject(UserStorage.Alerts, alerts);
        }

        public async static Task DeleteAlert(int index) {
            var localAlerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            localAlerts.RemoveAt(index);
            LocalStorageHelper.SaveObject(UserStorage.Alerts, localAlerts);
        }
    }
}
