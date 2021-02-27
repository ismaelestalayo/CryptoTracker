using CryptoTracker.Constants;
using CryptoTracker.Views;
using System;
using System.Threading.Tasks;

namespace CryptoTracker.Helpers {
    class FirstRunDialogHelper {

        internal static async Task ShowIfAppropriateAsync() {

            var NewUser = App._LocalSettings.Get<bool>(UserSettingsConstants.IsNewUser);

            if (NewUser) {
                var dialog = new FirstRunDialog();
                await dialog.ShowAsync();
                App._LocalSettings.Set(UserSettingsConstants.IsNewUser, false);
            }
        }
    }
}
