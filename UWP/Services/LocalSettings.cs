using UWP.Core.Constants;
using Windows.Storage;

namespace UWP.Services {
    /// <summary>
    /// Class for storing and retrieving user settings from
    /// application data settings storage.
    /// </summary>
    public class LocalSettings {

        public T Get<T>(string settingKey) {
            object result = ApplicationData.Current.LocalSettings.Values[settingKey];
            return result == null ? (T)UserSettingsConstants.Defaults[settingKey] : (T)result;
        }

        public void Set<T>(string settingKey, T value) {
            ApplicationData.Current.LocalSettings.Values[settingKey] = value;
        }
    }
}
