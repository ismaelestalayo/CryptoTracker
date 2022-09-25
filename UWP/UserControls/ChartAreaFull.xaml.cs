using Telerik.UI.Xaml.Controls.Chart;
using UWP.Models;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;


namespace UWP.UserControls {
    public sealed partial class ChartAreaFull : UserControl {

        public ChartAreaFull() {
            InitializeComponent();
        }

        
        public static readonly DependencyProperty ChartModelProperty = DependencyProperty.Register(
            nameof(ChartModel), typeof(ChartModel),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty ChartPointProperty = DependencyProperty.Register(
            nameof(ChartPoint), typeof(ChartPoint),
            typeof(ChartAreaFull), null);

        public static readonly DependencyProperty ShowVerticalAxisProperty = DependencyProperty.Register(
            nameof(ShowVerticalAxis), typeof(bool),
            typeof(ChartAreaFull), null);


        public ChartModel ChartModel {
            get => (ChartModel)GetValue(ChartModelProperty);
            set => SetValue(ChartModelProperty, value);
        }

        public ChartPoint ChartPoint {
            get => (ChartPoint)GetValue(ChartPointProperty);
            set => SetValue(ChartPointProperty, value);
        }

        public bool? ShowVerticalAxis {
            get => (bool)GetValue(ShowVerticalAxisProperty);
            set {
                SetValue(ShowVerticalAxisProperty, value);
                CartesianChartGrid.MajorLinesVisibility = (VerticalAxis.Visibility == Visibility.Visible) ?
                    GridLineVisibility.Y : GridLineVisibility.None;
            }
        }

        // ###################################################################
        private bool isZoomEnabled = true;
        public bool IsZoomEnabled {
            get => isZoomEnabled;
            set {
                isZoomEnabled = value;
                chartZoomMode = value ? ChartPanZoomMode.Both : ChartPanZoomMode.None;
                chartTrackBall = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ClearAnnotations()
            => Chart.Annotations.Clear();
        public void AddAnnotation(object val, bool isHorizontalLine, string label = "") {
            var dc = new DoubleCollection();
            dc.Add(15);
            Chart.Annotations.Add(new CartesianGridLineAnnotation() {
                Axis = isHorizontalLine ? Chart.VerticalAxis : Chart.HorizontalAxis,
                Label = label,
                Stroke = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)),
                StrokeThickness = 0.75,
                Value = val,
                MaxHeight = 100,
                Height = 100
            });
        }

        private ChartPanZoomMode chartZoomMode { get; set; } = ChartPanZoomMode.Horizontal;
        private Visibility chartTrackBall { get; set; } = Visibility.Visible;

        private void ChartTrackBall_Changed(FrameworkElement sender, DataContextChangedEventArgs args) {
            var point = ((DataPointInfo)sender.DataContext).DataPoint.DataItem as ChartPoint;
            ChartPoint = point;
        }

        private void Chart_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            => ChartPoint = new ChartPoint() { High = 0, Open = 0, Low = 0, Close = 0, Value = 0 };
    }
}
