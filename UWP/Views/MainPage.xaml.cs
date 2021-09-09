using CryptoTracker.Views;
using System;
using UWP.Core.Constants;
using UWP.Models;
using UWP.Shared.Constants;
using UWP.Shared.Interfaces;
using UWP.UserControls;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace UWP.Views {
    public sealed partial class MainPage : Page {

        private int CurrentTabIndex = 0;
        readonly UISettings uiSettings = new UISettings();

        private string Redirect = "";
        private string taskName = "BackgroundTask";
        private string taskEntryPoint = "UWP.Background.Tasks";

        // ###############################################################################################
        public MainPage() {
            this.InitializeComponent();

            // Clear the current tile
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();            

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();

                if (ColorConstants.CurrentThemeIsDark()){
                    statusBar.BackgroundColor = Color.FromArgb(255, 23, 23, 23); //31 31 31
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 255, 255, 255);
                } else {
                    statusBar.BackgroundColor = Color.FromArgb(255, 242, 242, 242); // 230
                    statusBar.BackgroundOpacity = 1;
                    statusBar.ForegroundColor = Color.FromArgb(255, 0, 0, 0);
                }

                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }

            /// Subscribe to light/dark theme change event
            uiSettings.ColorValuesChanged += ColorValuesChanged;

            ExtendAcrylicIntoTitleBar();

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed; ; ;
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e) {
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
                e.Handled = !TryGoBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e) {
            this.RegisterBackgroundTask();

            var param = e.Parameter.ToString();
            // if there's no param (tile or jump list) check user's startup page
            if (string.IsNullOrEmpty(param))
                param = App._LocalSettings.Get<string>(UserSettings.StartupPage);            
            
            if (param.StartsWith("/coin-"))
                Redirect = param.Split("-")[1];

            switch (param) {
                case "/Coins":
                    NavView.SelectedItem = NavView.MenuItems[1];
                    break;
                case "/News":
                    NavView.SelectedItem = NavView.MenuItems[2];
                    break;
                case "/Portfolio":
                    NavView.SelectedItem = NavView.MenuItems[3];
                    break;
                default:
                case null:
                case "/Home":
                    NavView.SelectedItem = NavView.MenuItems[0];
                    break;
            }
            ShowChangelog();

            await App.GetCoinList();
        }

        private async void RegisterBackgroundTask() {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy ||
                backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed) {
                foreach (var task in BackgroundTaskRegistration.AllTasks) {
                    if (task.Value.Name == taskName) {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                taskBuilder.Register();
            }
        }

        /// <summary>
        /// Show a notification with the changelog to new users.
        /// </summary>
        private void ShowChangelog() {
            var v = Package.Current.Id.Version;
            var currentVersion = $"{v.Major}.{v.Minor}.{v.Build}";
            
            var lastVersion = App._LocalSettings.Get<string>(UserSettings.LastVersion);
            if (currentVersion == lastVersion)
                return;

            vm.InfoBarTitle = $"Welcome to Crypto Tracker v{currentVersion}";
            vm.InfoBarMessage = "New in this version: \n";

            if (App._LocalSettings.Get<bool>(UserSettings.IsNewUser)) {
                App._LocalSettings.Set(UserSettings.IsNewUser, false);
                vm.InfoBarMessage += Changelogs.FormatChangelog(Changelogs.MajorChangelog);
            }
            else
                vm.InfoBarMessage += Changelogs.FormatChangelog(Changelogs.CurrentChangelog);

            vm.InfoBarOpened = true;
            App._LocalSettings.Set(UserSettings.LastVersion, currentVersion);
        }

        private async void ColorValuesChanged(UISettings sender, object args) {
            bool darkTheme = ColorConstants.CurrentThemeIsDark();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                ((Frame)Window.Current.Content).RequestedTheme = darkTheme ? ElementTheme.Dark : ElementTheme.Light;
            });
        }

        /// Extend acrylic into the title bar. 
        private void ExtendAcrylicIntoTitleBar() {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = Colors.Transparent;
            titleBar.InactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 150, 150, 150);
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveForegroundColor = Color.FromArgb(127, 150, 150, 150);
            Window.Current.SetTitleBar(CustomAppTitleBar);
        }

        /// #########################################################################################
        ///  Navigation View
        private async void NavView_Sync_Tapped(object sender, TappedRoutedEventArgs e) {
            SyncIcon.Visibility = Visibility.Collapsed;
            try {
                await((UpdatablePage)ContentFrame.Content).UpdatePage();
            }
            catch (Exception ex) {
                await new MessageDialog(ex.Message).ShowAsync();
            }

            SyncIcon.Visibility = Visibility.Visible;
        }

        public bool TryGoBack() {
            if (ContentFrame.CanGoBack) {
                ContentFrame.GoBack();
                return true;
            }
            return false;
        }

        private void NavView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args) {
            string source = "null";
            string selected = "null";

            if (args.IsSettingsSelected)
                return;

            if (((Frame)sender.Content).SourcePageType != null)
                source = ((Frame)sender.Content).SourcePageType.Name;
            
            selected = ((ContentControl)args.SelectedItem).Content.ToString();

            /// With ItemInvoked the navigation may have already been done
            /// so not to navigate twice, check the current page
            if (source != selected)
                PagesNavigation(selected);
        }
        
        private async void PagesNavigation(string toPage, bool samePage = false) {
            var dir = new SlideNavigationTransitionInfo();
            var page = typeof(Page);
            switch (toPage) {
                case "Home":
                    dir.Effect = SlideNavigationTransitionEffect.FromLeft;
                    page = typeof(Home);
                    CurrentTabIndex = 0;
                    break;
                case "Coins":
                    dir.Effect = (CurrentTabIndex > 1) ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
                    page = typeof(Coins);
                    CurrentTabIndex = 1;
                    break;
                case "News":
                    dir.Effect = (CurrentTabIndex > 2) ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
                    page = typeof(News);
                    CurrentTabIndex = 2;
                    break;
                case "Portfolio":
                    dir.Effect = (CurrentTabIndex > 3) ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
                    page = typeof(Portfolio);
                    CurrentTabIndex = 3;
                    break;
            }

            ///if it's the same page, override the default animation for one from the Bottom
            if (samePage)
                dir.Effect = SlideNavigationTransitionEffect.FromBottom;

            /// Redirect to a coin's page from a Live Tile (and clear Redirect as it's a one-time)
            if (toPage == "Home" && Redirect != "") {
                ContentFrame.Navigate(typeof(CoinDetails), Redirect, dir);
                Redirect = "";
            } else if (toPage == "Settings") {
                var settings = new SettingsDialog();
                await settings.ShowAsync();
            } else
                ContentFrame.Navigate(page, null, dir);
        }

        /// Hide NavigationView if navigating to the Compact Overlay view
        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e) {
            var toPage = e.SourcePageType.Name;
            App.CurrentPage = toPage;
            NavView.IsPaneVisible = (toPage == "CoinCompact") ? false : true;
            CustomAppTitleBar.Margin = (toPage == "CoinCompact") ? new Thickness(46, 0, 0, 0) : new Thickness(0);
        }

        /// #######################################################################################
        ///  AutoSuggest-Box
        private void AutoSuggestBox_TextChanged(AutoSuggestBox box, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
                box.ItemsSource = CoinAutoSuggestBox.FilterCoins(box);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            if (args.ChosenSuggestion != null)
                ContentFrame.Navigate(typeof(CoinDetails), ((SuggestionCoin)args.ChosenSuggestion).Symbol);
        }

        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e) {
            AutoSuggestBox box = sender as AutoSuggestBox;
            box.ItemsSource = CoinAutoSuggestBox.FilterCoins(box);
        }

        private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e) {
            //AutoSuggestBox.Text = "";
            if (((Frame)Window.Current.Content).ActualWidth < 720)
                AutoSuggestBox.Visibility = Visibility.Collapsed;
        }


        /// #######################################################################################
        ///  Search button
        private void NavView_Search_Tapped(object sender, TappedRoutedEventArgs e) {
            AutoSuggestBox.Visibility = Visibility.Visible;
            AutoSuggestBox.Focus(FocusState.Programmatic);
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args) {
            string selected;
            string source = (((Frame)sender.Content).SourcePageType).Name;
            
            selected = ((ContentControl)sender.SelectedItem).Content?.ToString();
            
            if (selected == null)
                selected = (args.IsSettingsInvoked) ? "Settings" : "Null";

            // CoinDetails and the WebView are loaded on the current NavigationViewTab
            // so the user can go back by clicking the same tab itself
            if (source == "CoinDetails" || source == "WebVieww" || selected == "Settings")
                PagesNavigation(selected, true);
            
            
        }

        private async void NavView_Settings_Tapped(object sender, TappedRoutedEventArgs e)
            => await new SettingsDialog().ShowAsync();

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
            => TryGoBack();
    }
}
