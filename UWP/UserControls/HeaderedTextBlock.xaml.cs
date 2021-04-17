using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace UWP.UserControls {
    public sealed partial class HeaderedTextBlock : UserControl {
        public HeaderedTextBlock() {
            InitializeComponent();
        }

        private object topTextBlock;
        public object TopTextBlock {
            get => topTextBlock;
            set => topTextBlock = value;
        }

        private TextBlock bottomTextBlock;
        public TextBlock BottomTextBlock {
            get => bottomTextBlock;
            set => bottomTextBlock = value;
        }

        public HorizontalAlignment Alignment { get; set; } = HorizontalAlignment.Left;
    }
}
