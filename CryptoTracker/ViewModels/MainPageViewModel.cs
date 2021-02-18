using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.Messaging.Messages;
using System;

namespace CryptoTracker.ViewModels {
    public class MainPageViewModel : ObservableRecipient {

        public MainPageViewModel() {
            IsActive = true;
        }

        protected override void OnActivated() {
            Messenger.Register<MainPageViewModel, NotificationMessage>(this, (r, m) => {
                InfoBarMessage = m.Value.Item2;
                InfoBarTitle = m.Value.Item3;
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

        private string infoBarTitle = "";
        public string InfoBarTitle {
            get => infoBarTitle;
            set => SetProperty(ref infoBarTitle, value);
        }

        private bool infoBarOpened = false;
        public bool InfoBarOpened {
            get => infoBarOpened;
            set => SetProperty(ref infoBarOpened, value);
        }
    }

    public sealed class NotificationMessage : ValueChangedMessage<Tuple<string, string, string>> {
        public NotificationMessage(Tuple<string, string, string> tuple) : base(tuple) {
        }
    }
}
