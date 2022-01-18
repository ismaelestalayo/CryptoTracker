using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;

namespace UWP.ViewModels {
    public partial class MainPageViewModel : ObservableRecipient {

        public MainPageViewModel() {
            IsActive = true;
        }

        protected override void OnActivated() {
            Messenger.Register<MainPageViewModel, NotificationMessage>(this, (r, m) => {
                InfoBarTitle = m.Value.Item1;
                InfoBarMessage = m.Value.Item2;
            });
        }

        // ####################################################################
        private string infoBarMessage = "";
        public string InfoBarMessage {
            get => infoBarMessage;
            set {
                SetProperty(ref infoBarMessage, value);
                InfoBarOpened = true;
            }
        }

        [ObservableProperty]
        private string infoBarTitle = "";

        [ObservableProperty]
        private bool infoBarOpened = false;
    }

    public sealed class NotificationMessage : ValueChangedMessage<Tuple<string, string>> {
        public NotificationMessage(Tuple<string, string> tuple) : base(tuple) {
        }
    }
}
