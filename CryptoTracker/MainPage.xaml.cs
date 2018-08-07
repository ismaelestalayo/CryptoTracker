using CryptoTracker.Helpers;
using CryptoTracker.Views;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace CryptoTracker {
    public sealed partial class MainPage : Page {

        private ObservableCollection<string> suggestions = new ObservableCollection<string>();

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

            FirstRunDialogHelper.ShowIfAppropriateAsync();

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

            Window.Current.SetTitleBar(AppTitle);
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        internal void UpdateButton_Click(object sender, RoutedEventArgs e) {

            switch (ContentFrame.SourcePageType.Name) {
                case "CoinDetails":
                    var p0 = (CoinDetails)ContentFrame.Content;
                    p0.UpdatePage();
                    break;
                case "Home":
                    var p2 = (Home)ContentFrame.Content;
                    p2.UpdateAllCards();
                    break;
                case "Portfolio":
                    var p1 = (Portfolio)ContentFrame.Content;
                    p1.UpdatePortfolio();
                    break;
            }

            LiveTile l = new LiveTile();
            l.UpdateLiveTile();
        }


        private void NavView_Loaded(object sender, RoutedEventArgs e) {
            // set the initial SelectedItem 
            NavView.SelectedItem = NavView.MenuItems[0];
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        // NavigationView
        private void NavView_Sync_Tapped(object sender, TappedRoutedEventArgs e) {
            UpdateButton_Click(sender, e);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args) {
            string name = ((ContentControl)args.SelectedItem).Content.ToString();
            pagesNavigation(name);
        }
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
            string name = args.InvokedItem.ToString();
            pagesNavigation(name);
        }
        private void pagesNavigation(string s) {
            switch (s) {
                case "Home":
                    ContentFrame.Navigate(typeof(Home));
                    mainTitle.Text = "Home";
                    mainTitleVal.Visibility  = Visibility.Collapsed;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;
                case "Top 100":
                    ContentFrame.Navigate(typeof(Top100));
                    mainTitle.Text = "Top 100";
                    mainTitleVal.Visibility  = Visibility.Collapsed;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;
                case "News":
                    ContentFrame.Navigate(typeof(News));
                    mainTitle.Text = "News";
                    mainTitleVal.Visibility  = Visibility.Collapsed;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;
                case "Portfolio":
                    ContentFrame.Navigate(typeof(Portfolio));
                    mainTitle.Text = "Portfolio";
                    mainTitleVal.Visibility  = Visibility.Visible;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;
                case "CoinDetails":
                    mainTitleVal.Visibility  = Visibility.Visible;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;

                case "Settings":
                    ContentFrame.Navigate(typeof(Settings));
                    mainTitle.Text = "Settings";
                    mainTitleVal.Visibility  = Visibility.Collapsed;
                    mainTitleDiff.Visibility = Visibility.Collapsed;
                    mainTitleLogo.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        // AUTO SUGGEST-BOX
        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
            // Only get results when it was a user typing, 
            // otherwise assume the value got filled in by TextMemberPath 
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
                sender.Text = sender.Text.ToUpper();
                suggestions.Clear();
                sender.ItemsSource = App.coinList.Where(x => x.StartsWith(sender.Text)).ToList();
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {
            CoinAutoSuggestBox.Text = args.SelectedItem.ToString();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args) {
            if (args.ChosenSuggestion != null) {
                ContentFrame.Navigate(typeof(CoinDetails), CoinAutoSuggestBox.Text);
            } else
                CoinAutoSuggestBox.Text = sender.Text;
        }

        private void AutoSuggestBox_GotFocus(object sender, Windows.UI.Xaml.RoutedEventArgs e) {
            AutoSuggestBox a = sender as AutoSuggestBox;
            a.ItemsSource = App.coinList.Where(x => x.StartsWith(a.Text)).ToList();
        }
    }
}
