using System.Collections.ObjectModel;
using System.Linq;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class AlertListGrouped : UserControl {
        public AlertListGrouped() {
            InitializeComponent();
        }

        public static readonly DependencyProperty AlertsProperty =
        DependencyProperty.Register(
            nameof(Alerts),
            typeof(ObservableCollection<Alert>),
            typeof(AlertListGrouped),
            null);

        public static readonly DependencyProperty CvsSourceProperty =
        DependencyProperty.Register(
            nameof(CvsSource),
            typeof(object),
            typeof(AlertListGrouped),
            null);

        public ObservableCollection<Alert> Alerts {
            get => (ObservableCollection<Alert>)GetValue(AlertsProperty);
            set {
                CvsSource = from alert in value group alert by alert.Crypto;
                SetValue(AlertsProperty, value);
            }
        }

        public object CvsSource {
            get => (object)GetValue(CvsSourceProperty);
            set => SetValue(CvsSourceProperty, value);
        }

        private void Delete_alert(object sender, RoutedEventArgs e) {
            var alert = ((FrameworkElement)sender).DataContext as Alert;
            Alerts.Remove(alert);
            CvsSource = from a in Alerts group a by a.Crypto;
        }
    }
}
