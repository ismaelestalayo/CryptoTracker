using System;
using Windows.ApplicationModel;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;


namespace CryptoTracker.Views {
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class FirstRunDialog : ContentDialog {

        public FirstRunDialog() {
            this.InitializeComponent();

            var package = Package.Current;
            var version = package.Id.Version;

            Title.Text = string.Format("Welcome to Crypto Tracker {0}.{1}.{2}", version.Major, version.Minor, version.Build);

            logo.Source = (new UISettings().GetColorValue(UIColorType.Background) == Colors.Black) ?
                new BitmapImage(new Uri("ms-appx:///Assets/AppIcon-L.png")) : new BitmapImage(new Uri("ms-appx:///Assets/AppIcon-D.png"));
        }
        
    }
}
