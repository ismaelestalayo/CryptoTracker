using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
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
