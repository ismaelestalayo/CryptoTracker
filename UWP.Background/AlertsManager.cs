using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Models;
using UWP.Services;

namespace UWP.Background {
    class AlertsManager {
        private static LocalSettings localSettings = new LocalSettings();

        internal static void test() {
            var currency = localSettings.Get<string>(UserSettings.Currency);
        }

        public static void CreateAlert(Alert alert) {
            var alerts = localSettings.Get<List<Alert>>(UserSettings.Alerts);
            alerts.Add(alert);
            localSettings.Set(UserSettings.Alerts, alerts);
        }

        public static void ChangeAlertState() {

        }
    }
}
