using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            try {
                var x = App.localSettings.Values["newUser"].Equals("false");

            } catch (Exception) {
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
                App.localSettings.Values["newUser"] = "false";
            }

        }
    }
}
