using UWP.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
    public sealed partial class SettingsDialog : ContentDialog {
        /// Store a refrence to Rectangle to later unregester the event handler
        private Rectangle _lockRectangle;
        public SettingsDialog() {
            this.InitializeComponent();

            this.SettingsFrame.Navigate(typeof(Settings));
        }

        protected override void OnApplyTemplate() {
            base.OnApplyTemplate();

            // get all open popups
            // normally there are 2 popups, one for your ContentDialog and one for Rectangle
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups) {
                if (popup.Child is Rectangle) {
                    _lockRectangle = popup.Child as Rectangle;
                    _lockRectangle.Tapped += OnLockRectangleTapped;
                }
            }
        }

        private void OnLockRectangleTapped(object sender, TappedRoutedEventArgs e) {
            this.Hide();
            _lockRectangle.Tapped -= OnLockRectangleTapped;
        }

        private void ContentFrame_Navigating(object sender, Windows.UI.Xaml.Navigation.NavigatingCancelEventArgs e) {

        }

    }
}
