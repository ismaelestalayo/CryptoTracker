using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CryptoTracker.UserControls {
    public sealed partial class DoubleTextBlock : UserControl {
        public DoubleTextBlock() {
            InitializeComponent();
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header), typeof(string), typeof(DoubleTextBlock), new PropertyMetadata(""));

        public string Header {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }


        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            nameof(Value), typeof(TextBlock), typeof(DoubleTextBlock), new PropertyMetadata(new TextBlock() { Text = "" }));

        public TextBlock Value {
            get => (TextBlock)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public static readonly DependencyProperty PrivateProperty = DependencyProperty.Register(
            nameof(Private), typeof(bool), typeof(DoubleTextBlock), new PropertyMetadata(false));

        public bool Private {
            get => (bool)GetValue(PrivateProperty);
            set => SetValue(PrivateProperty, value);
        }
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public double Spacing { get; set; } = 0;
    }
}
