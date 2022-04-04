using System;
using UWP.Core.Constants;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAbout : Page {

        private string Changelog;
        private string FooterVersion { get; set; }

        public SettingsAbout() {
            this.InitializeComponent();

            var version = Package.Current.Id.Version;
            FooterVersion = string.Format("Version {0}.{1}.{2}", version.Major, version.Minor, version.Build);

            var changelog = Changelogs.CurrentChangelog;
            Changelog = Changelogs.FormatChangelog(changelog);
        }

        private async void OpenInstallationFolder_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            var path = Windows.Storage.ApplicationData.Current.LocalFolder;
            await Launcher.LaunchFolderAsync(path);
        }
    }
}
