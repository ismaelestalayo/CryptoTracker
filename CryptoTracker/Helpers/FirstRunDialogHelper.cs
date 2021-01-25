using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            var _new = App.localSettings.Values["newUser"]?.ToString();

            if (_new == null) {
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
                App.localSettings.Values["newUser"] = "false";
            }

        }
    }
}
