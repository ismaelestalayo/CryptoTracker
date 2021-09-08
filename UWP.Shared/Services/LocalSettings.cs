using System;
using UWP.Core.Constants;
using Windows.Storage;

namespace UWP.Services {
    /// <summary>
    /// Class for storing and retrieving user settings from
    /// application data settings storage.
    /// </summary>
    public class LocalSettings {

        public T Get<T>(string settingKey) {
            try {
                object result = ApplicationData.Current.LocalSettings.Values[settingKey];
                return result == null ? (T)UserSettings.Defaults[settingKey] : (T)result;
            } catch (Exception ex) {
                return (T)UserSettings.Defaults[settingKey];
            }
        }

        public void Set<T>(string settingKey, T value) {
            ApplicationData.Current.LocalSettings.Values[settingKey] = value;
        }
    }
}
