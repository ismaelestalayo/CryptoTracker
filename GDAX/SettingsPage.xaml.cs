using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace GDAX {
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        public SettingsPage() {
            this.InitializeComponent();
        }

        private void ThemeToggled(object sender, RoutedEventArgs e) {

            RadioButton rb = sender as RadioButton;

            if (rb.Tag.ToString().Equals("Light"))
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;

            else if (rb.Tag.ToString().Equals("Dark"))
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;

        }

    }
}
