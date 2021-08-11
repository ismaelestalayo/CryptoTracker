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
            set {
                SetValue(IsCheckedProperty, value);
                Icon = value ? OnIcon : OffIcon;
            }
        }

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(IconElement), typeof(IconElement), null);
        private IconElement Icon {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public event RoutedEventHandler Click {
            add { ToggleButton.Click += value; }
            remove { ToggleButton.Click -= value; }
        }


        // On/Off icons do not need to be a dependancy
        private IconElement _onIcon;
        public IconElement OnIcon {
            get => _onIcon;
            set => _onIcon = value;
        }

        private static IconElement _offIcon;
        public IconElement OffIcon {
            get => _offIcon;
            set => _offIcon = value;
        }

        private string _tooltip = "";
        public string Tooltip {
            get => _tooltip;
            set => _tooltip = value;
        }

        // ##############################################################################
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
            => Icon = OnIcon;

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
            => Icon = OffIcon;

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            //Icon = IsChecked ? OnIcon : OffIcon;
            if (IsChecked)
                Icon = OnIcon;
        }
    }
}
