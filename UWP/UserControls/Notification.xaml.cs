using System;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace UWP.UserControls {
    public sealed partial class Notification : UserControl {

        public Notification() {
            this.InitializeComponent();
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(string), null);

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(string), null);

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(bool), null);

        public static readonly DependencyProperty MessageHeightProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(double), typeof(double), null);

        public string Title {
            get => (string)GetValue(TitleProperty);
            set {
                SetValue(TitleProperty, value);
                if (Message == "")
                    ThreadPoolTimer.CreateTimer(async (source) => {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                            IsOpen = false;
                        });
                    }, TimeSpan.FromSeconds(3));
            }
        }

        public string Message {
            get => (string)GetValue(MessageProperty);
            set {
                SetValue(MessageProperty, value);
                MessageHeight = (value == "") ? 0 : double.NaN;
            }
        }

        public bool IsOpen {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        internal double MessageHeight {
            get => (double)GetValue(MessageHeightProperty);
            set => SetValue(MessageHeightProperty, value);
        }

        private void InfoBar_Closed(Microsoft.UI.Xaml.Controls.InfoBar sender, Microsoft.UI.Xaml.Controls.InfoBarClosedEventArgs args) {
            IsOpen = false;
        }
    }
}
