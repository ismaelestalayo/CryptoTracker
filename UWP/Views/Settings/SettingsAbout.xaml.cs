using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAbout : Page {

        private string FooterVersion;

        public SettingsAbout() {
            this.InitializeComponent();

            var version = Package.Current.Id.Version;
            FooterVersion = string.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }

    }
}
