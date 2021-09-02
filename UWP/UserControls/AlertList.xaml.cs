using System.Collections.ObjectModel;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class AlertList : UserControl {
        public AlertList() {
            InitializeComponent();
        }

        public static readonly DependencyProperty AlertsProperty =
        DependencyProperty.Register(
            nameof(Alerts),
            typeof(ObservableCollection<Alert>),
            typeof(AlertList),
            null);

        public ObservableCollection<Alert> Alerts {
            get => (ObservableCollection<Alert>)GetValue(AlertsProperty);
            set => SetValue(AlertsProperty, value);
        }

        private void Delete_alert(object sender, RoutedEventArgs e) {
            var alert = ((FrameworkElement)sender).DataContext as Alert;
            Alerts.Remove(alert);
        }
    }
}
