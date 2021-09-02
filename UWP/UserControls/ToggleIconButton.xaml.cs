using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.UserControls {
    public sealed partial class ToggleIconButton : UserControl {

        public ToggleIconButton() {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(bool), new PropertyMetadata(false));

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public event RoutedEventHandler Click {
            add { ToggleButton.Click += value; }
            remove { ToggleButton.Click -= value; }
        }


        // On/Off icons do not need to be a dependancy
        private IconElement _checkedIcon = new SymbolIcon(Symbol.Like);
        public IconElement CheckedIcon {
            get => _checkedIcon;
            set => _checkedIcon = value;
        }
        private IconElement _uncheckedIcon = new SymbolIcon(Symbol.Dislike);
        public IconElement UncheckedIcon {
            get => _uncheckedIcon;
            set => _uncheckedIcon = value;
        }

        private string _tooltip = "";
        public string Tooltip {
            get => _tooltip;
            set => _tooltip = value;
        }

        // ##############################################################################

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
            => ToggleButton.Content = (ToggleButton.Content == CheckedIcon) ? UncheckedIcon : CheckedIcon;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
            => ToggleButton.Content = IsChecked ? CheckedIcon : UncheckedIcon;

    }
}
