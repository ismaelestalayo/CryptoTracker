using Telerik.UI.Xaml.Controls.Chart;
using UWP.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class ChartAreaFull : UserControl {

        public ChartAreaFull() {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty ChartModelProperty =
        DependencyProperty.Register(
            nameof(ChartModel),
            typeof(ChartModel),
            typeof(ChartAreaFull),
            null);

        public ChartModel ChartModel {
            get => (ChartModel)GetValue(ChartModelProperty);
            set => SetValue(ChartModelProperty, value);
        }

        public static readonly DependencyProperty ChartPointProperty =
        DependencyProperty.Register(
            nameof(ChartPoint),
            typeof(ChartPoint),
            typeof(ChartAreaFull),
            null);

        public ChartPoint ChartPoint {
            get => (ChartPoint)GetValue(ChartPointProperty);
            set => SetValue(ChartPointProperty, value);
        }

        public bool ChartZoom {
            set => chartZoom = value ? ChartPanZoomMode.Both : ChartPanZoomMode.None;
        }
        private ChartPanZoomMode chartZoom { get; set; } = ChartPanZoomMode.Horizontal;

        private void StackPanel_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args) {
            var point = ((DataPointInfo)sender.DataContext).DataPoint.DataItem as ChartPoint;
            ChartPoint = point;
        }

        private void Chart_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
            => ChartPoint = new ChartPoint() { High = 0, Open = 0, Low = 0, Close = 0, Value = 0 };
    }
}
