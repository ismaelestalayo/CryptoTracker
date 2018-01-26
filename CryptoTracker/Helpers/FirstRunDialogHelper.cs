using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            try {
                var x = App.localSettings.Values["2.2"].Equals("justUpdated");

            } catch (Exception) {
                App.localSettings.Values["2.2"] = "justUpdated";
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
            }

        }
    }
}
