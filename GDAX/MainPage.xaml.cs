using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace GDAX {

    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(Page2));
        }

        private void SettingsButton(object sender, RoutedEventArgs e) { 

            string x = MainFrame.Content.ToString();
            if (x.Equals("GDAX.Page2"))
                MainFrame.Navigate(typeof(SettingsPage));

            else if (x.Equals("GDAX.SettingsPage"))
                MainFrame.GoBack();
        }
    }
}
