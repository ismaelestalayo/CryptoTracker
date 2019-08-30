using CryptoTracker.Helpers;
using CryptoTracker.Views;
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

        private ObservableCollection<string> suggestions = new ObservableCollection<string>();
        private int CurrentTabIndex = 0;
        

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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            FirstRunDialogHelper.ShowIfAppropriateAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            // Extend acrylic
            ExtendAcrylicIntoTitleBar();
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e) {
            if (ContentFrame == null)
                return;

            // Navigate back if possible, and if the event has not already been handled .
            if (ContentFrame.CanGoBack && e.Handled == false) {
                e.Handled = true;
                ContentFrame.GoBack();
            }
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
            titleBar.ButtonInactiveForegroundColor = Color.FromArgb(255, 150, 150, 150);

            //Window.Current.SetTitleBar(AppTitle);
        }

        // #########################################################################################
        internal void UpdateButton_Click(object sender, RoutedEventArgs e) {

            switch (ContentFrame.SourcePageType.Name) {
                case "CoinDetails":
                    var p0 = (CoinDetails)ContentFrame.Content;
                    p0.UpdatePage();
                    break;
                case "Home":
                    var p2 = (Home)ContentFrame.Content;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    p2.UpdateAllCards();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    break;
                case "Portfolio":
                    var p1 = (Portfolio)ContentFrame.Content;
                    p1.UpdatePortfolio();
                    break;
            }

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
            if(args.IsSettingsSelected)
                PagesNavigation("Settings");
            else { 
                string name = ((ContentControl)args.SelectedItem).Content.ToString();
                PagesNavigation(name);
            }
        }
        
        private void PagesNavigation(string tag) {
            var direction = "fromleft";
            var page = typeof(Page);
            switch (tag) {
                case "Home":
                    direction = "fromleft";
                    page = typeof(Home);
                    CurrentTabIndex = 0;
                    break;
                case "Top 100":
                    direction = (CurrentTabIndex > 1) ? "fromleft" : "fromright";
                    page = typeof(Top100);
                    CurrentTabIndex = 1;
                    break;
                case "News":
                    direction = (CurrentTabIndex > 2) ? "fromleft" : "fromright";
                    page = typeof(News);
                    CurrentTabIndex = 2;
                    break;
                case "Portfolio":
                    direction = (CurrentTabIndex > 3) ? "fromleft" : "fromright";
                    page = typeof(Portfolio);
                    CurrentTabIndex = 3;
                    break;

                case "Settings":
                    direction = "fromright";
                    page = typeof(Settings);
                    CurrentTabIndex = 4;
                    break;
            }

            ContentFrame.Navigate(page, null,
                direction == "fromleft"
                    ? new SlideNavigationTransitionInfo() {Effect = SlideNavigationTransitionEffect.FromLeft}
                    : new SlideNavigationTransitionInfo() {Effect = SlideNavigationTransitionEffect.FromRight});
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
            findBtn.Visibility = Visibility.Visible;
            CoinAutoSuggestBox.Visibility = Visibility.Collapsed;
        }

        private void AutoSuggestBox_SizeChanged(object sender, SizeChangedEventArgs e) {
            if(e.PreviousSize == new Size(0, 0))
                CoinAutoSuggestBox.Focus(FocusState.Programmatic);
        }

        // #########################################################################################
        private List<SuggestionCoinList> FilterCoins(AutoSuggestBox box) {
            var filtered = App.coinList.Where(x => x.Name.Contains(box.Text) || x.FullName.Contains(box.Text)).ToList();
            List<SuggestionCoinList> list = new List<SuggestionCoinList>();
            foreach (JSONcoins coin in filtered) {
                list.Add(new SuggestionCoinList {
                    Icon = IconsHelper.GetIcon(coin.Name),
                    Name = coin.Name
                });
            }

            return list;
        }

        // #########################################################################################
        //  Search button
        private void NavView_Search_Tapped(object sender, TappedRoutedEventArgs e) {
            findBtn.Visibility = Visibility.Collapsed;
            CoinAutoSuggestBox.Visibility = Visibility.Visible;
            CoinAutoSuggestBox.Focus(FocusState.Programmatic);
        }

        
    }
}
