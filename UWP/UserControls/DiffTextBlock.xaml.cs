using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP.UserControls {
    public sealed partial class DiffTextBlock : UserControl {

        public DiffTextBlock() {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty DiffProperty =
            DependencyProperty.Register(
                nameof(Diff),
                typeof(double),
                typeof(double),
                null);

        public static readonly DependencyProperty DiffFgProperty =
            DependencyProperty.Register(
                nameof(DiffFg),
                typeof(SolidColorBrush),
                typeof(SolidColorBrush),
                null);

        public static readonly DependencyProperty DiffArrowProperty =
            DependencyProperty.Register(
                nameof(DiffArrowProperty),
                typeof(string),
                typeof(string),
                null);

        private string DiffArrow {
            get => (string)GetValue(DiffArrowProperty);
            set => SetValue(DiffArrowProperty, value);
        }

        public double Diff {
            get => (double)GetValue(DiffProperty);
            set {
                DiffArrow = (value < 0) ? "▼" : "▲";
                SetValue(DiffProperty, Math.Abs(value));
            }
        }

        public SolidColorBrush DiffFg {
            get => (SolidColorBrush)GetValue(DiffFgProperty);
            set => SetValue(DiffFgProperty, value);
        }

    }
}
