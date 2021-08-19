using UWP.Core.Constants;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsChangelog : Page {

        public SettingsChangelog() {
            InitializeComponent();
            Loaded += SettingsChangelog_Loaded;
        }

        private void SettingsChangelog_Loaded(object sender, RoutedEventArgs e) {
            var version = Package.Current.Id.Version;

            foreach (var changelog in Changelogs.LatestChangelogs) {
                var changes = Changelogs.FormatChangelog(changelog.Value);
                var s = new StackPanel() {
                    Children = {
                        new TextBlock(){ Text = changelog.Key, Style = Resources["SettingsSectionSubtitle"] as Style },
                        new TextBlock(){ Text = changes }
                    }
                };
                ChangeLogList.Children.Add(s);
            }
        }


        // ###############################################################################################

    }
}
