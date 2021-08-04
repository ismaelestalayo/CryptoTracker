using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.UserControls {
    public sealed partial class ToggleIconButton : UserControl {

        public ToggleIconButton() {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(bool), null);

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        // On/Off icons do not need to be a dependancy
        private string _onIcon = "";
        public string OnIcon {
            get => _onIcon;
            set => _onIcon = value;
        }

        private string _offIcon = "";
        public string OffIcon {
            get => _offIcon;
            set => _offIcon = value;
        }

        // ##############################################################################
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
            => ToggleButton.Content = OnIcon;

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
            => ToggleButton.Content = OffIcon;
    }
}
