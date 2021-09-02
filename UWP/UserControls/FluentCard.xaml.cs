using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;


namespace CryptoTracker.UserControls {
    [ContentProperty(Name = nameof(SettingsActionableElement))]
    public sealed partial class FluentCard : UserControl {
        public FrameworkElement SettingsActionableElement { get; set; }

        public static readonly DependencyProperty ExpandableContentProperty = DependencyProperty.Register(
          "ExpandableContent",
          typeof(FrameworkElement),
          typeof(FluentCard),
          new PropertyMetadata(null)
        );

        public FrameworkElement ExpandableContent {
            get => (FrameworkElement)GetValue(ExpandableContentProperty);
            set => SetValue(ExpandableContentProperty, value);
        }

        public static readonly DependencyProperty TitleContentProperty = DependencyProperty.Register(
          "TitleContent",
          typeof(FrameworkElement),
          typeof(FluentCard),
          new PropertyMetadata(null)
        );

        public FrameworkElement TitleContent {
            get => (FrameworkElement)GetValue(TitleContentProperty);
            set => SetValue(TitleContentProperty, value);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
          "Title",
          typeof(string),
          typeof(FluentCard),
          new PropertyMetadata(null)
        );

        public string Title {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
          "Description",
          typeof(string),
          typeof(FluentCard),
          new PropertyMetadata(null)
        );

        public string Description {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
          "Icon",
          typeof(IconElement),
          typeof(FluentCard),
          new PropertyMetadata(null)
        );

        public IconElement Icon {
            get => (IconElement)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IsClickableProperty = DependencyProperty.Register(
          "IsClickable",
          typeof(bool),
          typeof(FluentCard),
          new PropertyMetadata(false)
        );

        public bool IsClickable {
            get => (bool)GetValue(IsClickableProperty);
            set => SetValue(IsClickableProperty, value);
        }

        //
        // Summary:
        //     Occurs when a button control is clicked.
        public event RoutedEventHandler Click;

        public FluentCard() {
            InitializeComponent();
        }

        private void ActionableButton_Click(object sender, RoutedEventArgs e) {
            Click?.Invoke(this, e);
        }

        private void Expander_Expanding(Microsoft.UI.Xaml.Controls.Expander sender, Microsoft.UI.Xaml.Controls.ExpanderExpandingEventArgs args) {
            Click?.Invoke(this, new RoutedEventArgs());
        }

        private void Expander_Collapsed(Microsoft.UI.Xaml.Controls.Expander sender, Microsoft.UI.Xaml.Controls.ExpanderCollapsedEventArgs args) {
            Click?.Invoke(this, new RoutedEventArgs());
        }
    }
}
