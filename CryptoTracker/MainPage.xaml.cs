using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace CryptoTracker {
    public sealed partial class MainPage : Page {

        private readonly ObservableCollection<string> suggestions = new ObservableCollection<string>();
        private int CurrentTabIndex = 0;
        readonly UISettings uiSettings = new UISettings();

        // ###############################################################################################
        public MainPage() {
            this.InitializeComponent();

            // Clear the current tile
            //TileUpdateManager.CreateTileUpdaterForApplication().Clear();            

            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop") {
                CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            }

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = StatusBar.GetForCurrentView();

                if (App.localSettings.Values["Theme"].Equals("Dark")) {
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

            // Subscribe to light/dark theme change event
            uiSettings.ColorValuesChanged += ColorValuesChanged;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            FirstRunDialogHelper.ShowIfAppropriateAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Extend acrylic
            ExtendAcrylicIntoTitleBar();
        }

        private void ColorValuesChanged(UISettings sender, object args) {
            if (App.localSettings.Values["Theme"].Equals("Windows")){
                var color = uiSettings.GetColorValue(UIColorType.Background);
                switch (color.ToString()) {
                    case "#FF000000":
                        ChangeTheme("Dark");
                        break;
                    case "#FFFFFFFF":
                        ChangeTheme("Light");
                        break;
                }
            }
        }

        private async void ChangeTheme(string theme) {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                ((Frame)Window.Current.Content).RequestedTheme = theme.Equals("Dark") ? ElementTheme.Dark : ElementTheme.Light;
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

            //Window.Current.SetTitleBar(AppTitle);
        }

        // #########################################################################################
        internal async void UpdateButton_Click(object sender, RoutedEventArgs e) {
            SyncIcon.Visibility = Visibility.Collapsed;
            switch (ContentFrame.SourcePageType.Name) {
                case "CoinDetails":
                    var p0 = (CoinDetails)ContentFrame.Content;
                    p0.UpdatePage();
                    break;
                case "Home":
                    var p2 = (Home)ContentFrame.Content;
                    await p2.UpdateAllCards();
                    break;
                case "Portfolio":
                    var p1 = (Portfolio)ContentFrame.Content;
                    p1.UpdatePortfolio();
                    break;
                case "News":
                    var p3 = (News)ContentFrame.Content;
                    p3.GetNews();
                    break;
            }

            SyncIcon.Visibility = Visibility.Visible;
            //LiveTile l = new LiveTile();
            //l.UpdateLiveTile();
        }

        // #########################################################################################
        //  Navigation View
        private void NavView_Loaded(object sender, RoutedEventArgs e) {
            // set the initial SelectedItem 
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        private void NavView_Sync_Tapped(object sender, TappedRoutedEventArgs e) {
            UpdateButton_Click(sender, e);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            string source = "null";
            string selected = "null";

            if (((Frame)sender.Content).SourcePageType != null)
                source = (((Frame)sender.Content).SourcePageType).Name;
            
            if (args.IsSettingsSelected)
                selected = "Settings";
            else 
                selected = ((ContentControl)args.SelectedItem).Content.ToString();
            
            // With ItemInvoked the navigation may have already been done
            // so not to navigate twice, check the current page
            if (source != selected)
                PagesNavigation(selected);
        }
        
        private void PagesNavigation(string toPage, bool samePage = false) {
            var dir = new SlideNavigationTransitionInfo();
            var page = typeof(Page);
            switch (toPage) {
                case "Home":
                    dir.Effect = SlideNavigationTransitionEffect.FromLeft;
                    page = typeof(Home);
                    CurrentTabIndex = 0;
                    break;
                case "Top 100":
                    dir.Effect = (CurrentTabIndex > 1) ? SlideNavigationTransitionEffect.FromLeft : SlideNavigationTransitionEffect.FromRight;
                    page = typeof(Top100);
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

                case "Settings":
                    dir.Effect = SlideNavigationTransitionEffect.FromRight;
                    page = typeof(Settings);
                    CurrentTabIndex = 4;
                    break;
            }

            // if it's the same page, override the default animation for one from the Bottom
            if (samePage) { 
                dir.Effect = SlideNavigationTransitionEffect.FromBottom;
            }

            ContentFrame.Navigate(page, null, dir);
            
        }

        // #########################################################################################
        //  AutoSuggest-Box
        private void AutoSuggestBox_TextChanged(AutoSuggestBox box, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                box.Text = box.Text.ToUpper();
                suggestions.Clear();

                box.ItemsSource = FilterCoins(box);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {
            CoinAutoSuggestBox.Text = ((SuggestionCoinList)args.SelectedItem).Name;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            if (args.ChosenSuggestion != null) {
                ContentFrame.Navigate(typeof(CoinDetails), ((SuggestionCoinList)args.ChosenSuggestion).Name);
            }
            
            CoinAutoSuggestBox.Text = "";
        }

        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e) {
            //AutoSuggestBox box = sender as AutoSuggestBox;
            //box.ItemsSource = FilterCoins(box);
        }

        private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e) {
            CoinAutoSuggestBox.Text = "";
            CoinAutoSuggestBox.Visibility = Visibility.Collapsed;
        }

        private void AutoSuggestBox_SizeChanged(object sender, SizeChangedEventArgs e) {
            if(e.PreviousSize == new Size(0, 0))
                CoinAutoSuggestBox.Focus(FocusState.Programmatic);
        }

        // #########################################################################################
        private List<SuggestionCoinList> FilterCoins(AutoSuggestBox box) {
            var filtered = App.coinList.Where(x => x.symbol.Contains(box.Text)).ToList(); // || x.FullName.Contains(box.Text)
            List<SuggestionCoinList> list = new List<SuggestionCoinList>();
            foreach (CoinBasicInfo coin in filtered) {
                list.Add(new SuggestionCoinList {
                    Icon = IconsHelper.GetIcon(coin.symbol),
                    Name = coin.symbol
                });
            }

            return list;
        }

        // #########################################################################################
        //  Search button
        private void NavView_Search_Tapped(object sender, TappedRoutedEventArgs e) {
            CoinAutoSuggestBox.Visibility = Visibility.Visible;
            CoinAutoSuggestBox.Focus(FocusState.Programmatic);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
            string selected;
            string source = (((Frame)sender.Content).SourcePageType).Name;
            
            selected = ((ContentControl)sender.SelectedItem).Content?.ToString();
            
            if (selected == null) {
                if (args.IsSettingsInvoked)
                    selected = "Settings";
                else
                    selected = "Null";
            }

            // CoinDetails and the WebView are loaded on the current NavigationViewTab
            // so the user can go back by clicking the same tab itself
            if (source == "CoinDetails" || source == "WebVieww")
                PagesNavigation(selected, true);
            
            
        }
    }
}
