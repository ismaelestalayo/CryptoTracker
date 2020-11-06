using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            var newUser = App.localSettings.Values["newUser"];

            if (newUser == null || newUser.ToString() != "false") {
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
                App.localSettings.Values["newUser"] = "false";
            }

        }
    }
}
