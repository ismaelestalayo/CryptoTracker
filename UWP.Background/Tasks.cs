using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWP.Core.Constants;
using UWP.Helpers;
using UWP.Models;
using UWP.Services;
using Windows.ApplicationModel.Background;
using Windows.UI.StartScreen;
using Windows.UI.Xaml.Media.Imaging;

namespace UWP.Background {
    public sealed class Tasks : XamlRenderingBackgroundTask {

        protected override async void OnRun(IBackgroundTaskInstance taskInstance) {

            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            /// Register services (Background task can't access services from the UWP)
            Ioc.Default.ConfigureServices(
                new ServiceCollection()
                .AddSingleton(RestService.For<ICryptoCompare>("https://min-api.cryptocompare.com/"))
                .AddTransient<LocalSettings>()
                .BuildServiceProvider());



            // TODO: update Primary Tile
            var tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles) {
                try {
                    await LiveTileUpdater.AddSecondaryTile(tile.TileId);
                }
                catch (Exception ex) {
                    var z = ex.Message;
                }
            }

            // TODO: check price alerts
            await CheckAlerts();

            deferral.Complete();
        }

        private async Task CheckAlerts() {
            var localAlerts = await LocalStorageHelper.ReadObject<List<Alert>>(UserStorage.Alerts);
            var localSettings = new LocalSettings();
            var currency = localSettings.Get<string>(UserSettings.Currency);

            var enabledAlerts = localAlerts.Where(x => x.Enabled).ToList();
            var alerts = enabledAlerts.GroupBy(x => x.Crypto);

            /// More efficient: get price once for all alerts of the same crypto
            foreach (var alert in alerts) {
                var data = await Ioc.Default.GetService<ICryptoCompare>().GetPrice(alert.Key, currency);
                double price = double.Parse(data.Split(":")[1].Replace("}", ""));

                /// Go check each alert, and if the user is notified, disable it not to spam
                foreach (var a in alert)
                    if (CheckAlert(a, price))
                        localAlerts[localAlerts.IndexOf(a)].Enabled = false;
            }
            await LocalStorageHelper.SaveObject(UserStorage.Alerts, localAlerts);
        }

        private bool CheckAlert(Alert alert, double price) {
            string header = "";
            string subheader = $"Currently at {price}{alert.CurrencySymbol}";
            switch (alert.Mode.ToLowerInvariant()) {
                case "below":
                    if (price < alert.Threshold) {
                        header = $"📉 {alert.Crypto} {alert.Mode} {alert.Threshold}{alert.CurrencySymbol}";
                        ToastGenerator.SendAlert(header, subheader, alert.Crypto);
                        return true;
                    }
                    return false;
                case "above":
                    if (price >= alert.Threshold) {
                        header = $"🚀 {alert.Crypto} {alert.Mode} {alert.Threshold}{alert.CurrencySymbol}";
                        ToastGenerator.SendAlert(header, subheader, alert.Crypto);
                        alert.Enabled = false;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
    }
}
