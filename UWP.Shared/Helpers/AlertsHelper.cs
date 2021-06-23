using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;

namespace UWP.Shared.Helpers {
    public class AlertsHelper {
        public async static Task<List<Alert>> GetAlerts()
            => await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);

        public async static Task<ObservableCollection<Alert>> GetCryptoAlerts(string crypto) {
            var allAlerts = await GetAlerts();
            var alerts = allAlerts.Where(x => x.Crypto == crypto).ToList();
            return new ObservableCollection<Alert>(alerts);
        }

        public async static void UpdateOneCryptoAlerts(string crypto, ObservableCollection<Alert> alerts) {
            var localAlerts = await GetAlerts();
            foreach (var alert in localAlerts.ToList())
                if (alert.Crypto == crypto)
                    localAlerts.Remove(alert);
            localAlerts.AddRange(alerts);
            await LocalStorageHelper.SaveObject(UserStorage.Alerts, localAlerts);
        }

        public async static Task DeleteAlert(int index) {
            var localAlerts = await GetAlerts();
            localAlerts.RemoveAt(index);
            await LocalStorageHelper.SaveObject(UserStorage.Alerts, localAlerts);
        }
    }
}
