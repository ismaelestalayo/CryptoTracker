using Windows.UI.Xaml;

namespace CryptoTracker.Helpers {
    static class Converters {
        public static Visibility InvertVisibility(Visibility value) => value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
    }
}
