using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Chart;
using UWP.Converters;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP.UserControls {
    public sealed partial class ChartCandles : UserControl {

        public ChartCandles() {
            InitializeComponent();
        }

        // ###################################################################
        public static readonly DependencyProperty ChartModelProperty = DependencyProperty.Register(
            nameof(ChartModel), typeof(ChartModel),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty ChartPointProperty = DependencyProperty.Register(
            nameof(ChartPoint), typeof(ChartPoint),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty AlertsProperty = DependencyProperty.Register(
            nameof(Alerts), typeof(ObservableCollection<Alert>),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty PurchasesProperty = DependencyProperty.Register(
            nameof(Purchases), typeof(ObservableCollection<PurchaseModel>),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty ShowAlertsProperty = DependencyProperty.Register(
            nameof(ShowAlerts), typeof(bool),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty ShowPurchasesProperty = DependencyProperty.Register(
            nameof(ShowPurchases), typeof(bool),
            typeof(ChartAreaFull), null);

        public ChartModel ChartModel {
            get => (ChartModel)GetValue(ChartModelProperty);
            set => SetValue(ChartModelProperty, value);
        }

        public ChartPoint ChartPoint {
            get => (ChartPoint)GetValue(ChartPointProperty);
            set => SetValue(ChartPointProperty, value);
        }

        public ObservableCollection<Alert> Alerts {
            get => (ObservableCollection<Alert>)GetValue(AlertsProperty) ?? new ObservableCollection<Alert>();
            set {
                SetValue(AlertsProperty, value);
                DrawAllAnnotations();
            }
        }

        public ObservableCollection<PurchaseModel> Purchases {
            get => (ObservableCollection<PurchaseModel>)GetValue(PurchasesProperty) ?? new ObservableCollection<PurchaseModel>();
            set {
                SetValue(PurchasesProperty, value);
                DrawAllAnnotations();
            }
        }

        public bool? ShowAlerts {
            get => (bool)GetValue(ShowAlertsProperty);
            set {
                SetValue(ShowAlertsProperty, value);
                DrawAllAnnotations();
            }
        }

        public bool? ShowPurchases {
            get => (bool)GetValue(ShowPurchasesProperty);
            set {
                SetValue(ShowPurchasesProperty, value);
                DrawAllAnnotations();
            }
        }

        // ###################################################################
        private void DrawAllAnnotations() {
            Chart.Annotations.Clear();
            // draw annotations for alerts (vertical axis = horizontal line)
            if (ShowAlerts.Value)
                foreach (var alert in Alerts)
                    Chart.Annotations.Add(new CartesianGridLineAnnotation() {
                        Axis = Chart.VerticalAxis,
                        Label = new NumberRounder().Convert(alert.Threshold, null, null, null).ToString() + App.currencySymbol,
                        LabelDefinition = new ChartAnnotationLabelDefinition() {
                            HorizontalAlignment = HorizontalAlignment.Left,
                            HorizontalOffset = 5,
                            Location = ChartAnnotationLabelLocation.Top
                        },
                        Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128)),
                        StrokeThickness = 0.75,
                        Opacity = 0.5,
                        Value = alert.Threshold
                    });
            // draw annotations for purchases (horizontal axis = vertical line)
            if (ShowPurchases.Value)
                foreach (var purchase in Purchases)
                    Chart.Annotations.Add(new CartesianGridLineAnnotation() {
                        Axis = Chart.HorizontalAxis,
                        Label = purchase.Type[0].ToString().ToUpper(),
                        Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 128, 128, 128)),
                        StrokeDashArray = new DoubleCollection() { 20, 10 },
                        StrokeThickness = 0.75,
                        Opacity = 0.5,
                        Value = purchase.Date.DateTime
                    });
        }

        // ###################################################################
        private void ChartTrackBall_Changed(FrameworkElement sender, DataContextChangedEventArgs args) {
            var point = ((DataPointInfo)sender.DataContext).DataPoint.DataItem as ChartPoint;
            ChartPoint = point;
        }

        private void Chart_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            => ChartPoint = new ChartPoint() { High = 0, Open = 0, Low = 0, Close = 0, Value = 0 };
    }
}
