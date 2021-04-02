using System.Collections.Generic;
using UWP.Core.Constants;
using UWP.Models;
using Windows.UI.Xaml.Controls;

namespace UWP.Views {
    public sealed partial class SettingsAlerts : Page {

        private List<Alert> Alerts;

        public SettingsAlerts() {
            this.InitializeComponent();

            Alerts = App._LocalSettings.Get<List<Alert>>(UserSettings.Alerts);
        }


        // ###############################################################################################

        
    }
}
