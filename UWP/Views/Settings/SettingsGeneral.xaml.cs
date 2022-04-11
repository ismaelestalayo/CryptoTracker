using System;
using System.Collections.Generic;
using System.Linq;
using UWP.Core.Constants;
using UWP.Shared.Constants;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsGeneral : Page {

        private Dictionary<string, string> AvailableCurrencies = Currencies.CurrencySymbol;

        public SettingsGeneral() {
            InitializeComponent();
            Loaded += SettingsGeneral_Loaded;
        }

        private async void SettingsGeneral_Loaded(object sender, RoutedEventArgs e) {
            vm.AutoRefresh = App._LocalSettings.Get<string>(UserSettings.AutoRefresh);
            vm.Currency = App._LocalSettings.Get<string>(UserSettings.Currency);
            vm.StartupPage = App._LocalSettings.Get<string>(UserSettings.StartupPage).Replace("/", "");
            vm.Timespan = App._LocalSettings.Get<string>(UserSettings.Timespan);

            var task = await StartupTask.GetAsync("CE0D17DE-AC9B-4B2D-AE14-FFACDE33BF00");
            vm.CanOpenInLogin = new[] { StartupTaskState.Disabled, StartupTaskState.Enabled }.Contains(
                task.State);
            vm.OpenInLogin = new[]{ StartupTaskState.Enabled, StartupTaskState.EnabledByPolicy}.Contains(
                task.State);

            StartupToggleSwitch.Toggled += Startup_Toggled;
        }

        // ###############################################################################################
        private void StartupPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => App._LocalSettings.Set(UserSettings.StartupPage, $"/{vm.StartupPage}");

        private void Timespan_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => App._LocalSettings.Set(UserSettings.Timespan, vm.Timespan);

        private async void Startup_Toggled(object sender, RoutedEventArgs e) {
            var startupTask = await StartupTask.GetAsync("CE0D17DE-AC9B-4B2D-AE14-FFACDE33BF00");

            if (((ToggleSwitch)sender).IsOn) {
                var result = await startupTask.RequestEnableAsync();
                if (result != StartupTaskState.Enabled) {
                    vm.OpenInLogin = false;
                    vm.CanOpenInLogin = false;
                }
            }
            else {
                startupTask.Disable();
            }
        }

        private async void StartupInfoBarBtn_Click(object sender, RoutedEventArgs e)
            => await Launcher.LaunchUriAsync(new Uri("ms-settings:startupapps"));
    }
}
