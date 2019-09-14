using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace CryptoTracker.Views {
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class FirstRunDialog : ContentDialog {

        public FirstRunDialog() {
            this.InitializeComponent();

            var package = Package.Current;
            var version = package.Id.Version;

            Title.Text += string.Format(" {0}.{1}.{2}", version.Major, version.Minor, version.Revision);
        }
        
    }
}
