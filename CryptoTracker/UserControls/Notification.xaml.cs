using System;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CryptoTracker.UserControls {
    public sealed partial class Notification : UserControl {

        private ThreadPoolTimer PeriodicTimer;
        public Notification() {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(string),
            null);

        public static readonly DependencyProperty MessageProperty =
        DependencyProperty.Register(
            nameof(Message),
            typeof(string),
            typeof(string),
            null);

        public static readonly DependencyProperty IsOpenProperty =
        DependencyProperty.Register(
            nameof(IsOpen),
            typeof(bool),
            typeof(bool),
            null);

        public string Title {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Message {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public bool IsOpen {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private bool temporary = true;
        public bool Temporary {
            get => temporary;
            set {
                temporary = value;
                if (temporary)
                    PeriodicTimer = ThreadPoolTimer.CreateTimer(async (source) => {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                            if (IsOpen)
                                IsOpen = false;
                        });
                    }, TimeSpan.FromSeconds(4));
            }
        }

        private void InfoBar_Closed(Microsoft.UI.Xaml.Controls.InfoBar sender, Microsoft.UI.Xaml.Controls.InfoBarClosedEventArgs args) {
            IsOpen = false;
        }
    }
}
