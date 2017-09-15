using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

using CoinBase.Helpers;
using CoinBase.Views;

namespace CoinBase.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            try {
                var x = App.localSettings.Values["firstRun"].Equals("notFirstRun");

            } catch (Exception) {
                App.localSettings.Values["firstRun"] = "notFirstRun";
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }

        }
    }
}
