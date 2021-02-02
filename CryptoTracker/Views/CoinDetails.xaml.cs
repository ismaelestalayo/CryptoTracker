using CryptoTracker.APIs;
using CryptoTracker.Helpers;
using CryptoTracker.Views;
using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {

    public class ChartLine {
        public PropertyNameDataPointBinding Datee { get; set; }
        public PropertyNameDataPointBinding Valuee { get; set; }
    }

    public class CoinDataWrapper : INotifyPropertyChanged {
        private CoinData _cd;
        public CoinData cd {
            get { return _cd; }
            set { _cd = value; OnPropertyChanged("cd"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed partial class CoinDetails : Page {
        private static int limit = 60;
        private static string timeSpan = "week";
        private static ThreadPoolTimer PeriodicTimer;

        internal string crypto { get; set; }
        internal string currency = App.currency;
        internal string currencySymbol = App.currencySymbol;
        public CoinDataWrapper cdw { get; set; }

        public CoinDetails() {
            this.InitializeComponent();

            cdw = new CoinDataWrapper();

            if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                CompactOverlay_btn.Visibility = Visibility.Visible;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            try {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("toCoinDetails");
                if (animation != null)
                    animation.TryStart(PriceChart, new UIElement[]{ BottomCards } );
                
                
                // Page title
                crypto = e.Parameter?.ToString() ?? "NULL";

                var currentCoin = App.coinList.Find(x => x.symbol == crypto);
                mainTitle.Text = string.Format("{0} ({1})", currentCoin.name, currentCoin.symbol);

                try {
                    mainTitleLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon" + crypto.ToUpper(CultureInfo.InvariantCulture) + ".png"));
                } catch(Exception) {
                    mainTitleLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/iconNULL.png"));
                    Analytics.TrackEvent(string.Format("Missing icon: {0}", crypto));
                }
                mainTitleLogo.Visibility = Visibility.Visible;

                try {
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources[crypto.ToUpper(CultureInfo.InvariantCulture) + "_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources[crypto.ToUpper(CultureInfo.InvariantCulture) + "_colorT"]).Color;
                } catch {
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["Main_WhiteBlack"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["Main_WhiteBlackT"]).Color;
                }

                FavIcon.Content = App.pinnedCoins.Contains(crypto.ToUpper(CultureInfo.InvariantCulture)) ? "\uEB52" : "\uEB51";

                InitValues();
            }
            catch (Exception ex){
                var message = "There was an error loading that coin. Try again later.";
                new MessageDialog(message).ShowAsync();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e) {
            if (PeriodicTimer != null)
                PeriodicTimer.Cancel();
        }

        private async void InitValues() {

            CoinBasicInfo coin = App.coinList.Find(x => x.symbol == crypto);
            cdw.cd = await API_CoinGecko.GetCoin(coin.name);

            TimeSpan period = TimeSpan.FromSeconds(30);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour" && this.Frame.SourcePageType.Name == "CoinDetails")
                        TimerangeButton_Click(r, null);
                });
            }, period);

            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                TimerangeButton_Click(r, null);

            } catch (Exception) {
                LoadingControl.IsLoading = false;
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        // #########################################################################################
        // #########################################################################################
        // #########################################################################################
        internal async void UpdatePage() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;

            await UpdateCoin();
            verticalAxis.Minimum = GraphHelper.GetMinimum(App.historic);
            verticalAxis.Maximum = GraphHelper.GetMaximum(App.historic);
            dateTimeAxis = App.AdjustAxis(dateTimeAxis, timeSpan);
            await Get24Volume();
            //CryptoCompare.GetExchanges(crypto);
        }

        // #########################################################################################
        private async Task UpdateCoin() {

            var price = await CryptoCompare.GetPriceAsync(crypto);
            mainTitleVal.Text = price.ToString() + App.currencySymbol;
            mainTitleVal.Visibility  = Visibility.Visible;
            mainTitleDiff.Visibility = Visibility.Visible;

            switch (timeSpan) {
                case "hour":
                case "day":
                    await App.GetHisto(crypto, "minute", limit);
                    break;

                case "week":
                case "month":
                    await App.GetHisto(crypto, "hour", limit);
                    break;

                case "year":
                    await App.GetHisto(crypto, "day", limit);
                    break;

                case "all":
                    await App.GetHisto(crypto, "day", 0);
                    break;
            }

            List<ChartData> data = new List<ChartData>();
            for (int i = 0; i < App.historic.Count; ++i) {
                ChartData obj = new ChartData {
                    Date   =  App.historic[i].DateTime,
                    Value  = (App.historic[i].Low + App.historic[i].High) / 2,
                    Low    =  App.historic[i].Low,
                    High   =  App.historic[i].High,
                    Open   =  App.historic[i].Open,
                    Close  =  App.historic[i].Close,
                    Volume =  App.historic[i].Volumefrom
                };
                data.Add(obj);
            }

            float oldestPrice = App.historic[0].Close;
            float newestPrice = App.historic[App.historic.Count - 1].Close;
            float d = (float)Math.Round((newestPrice / oldestPrice - 1) * 100, 2);

            if (d < 0) {
                mainTitleDiff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                d = Math.Abs(d);
                mainTitleDiff.Text = "▼" + d.ToString() + "%";
            } else {
                mainTitleDiff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                mainTitleDiff.Text = "▲" + d.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)PriceChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        
        async private Task Get24Volume() {
            await App.GetHisto(crypto, "hour", 24);

            List<ChartData> data = new List<ChartData>();
            for (int i = 0; i < 24; i++) {
                data.Add(new ChartData() {
                    Date   = App.historic[i].DateTime,
                    Volume = App.historic[i].Volumefrom
                });
            }
            this.volumeChart.DataContext = data;
        }

        // #########################################################################################
        private void TimerangeButton_Click(object sender, RoutedEventArgs e) {
            LoadingControl.IsLoading = true;
            RadioButton btn = sender as RadioButton;

            switch (btn.Content) {
                case "hour":
                    timeSpan = "hour";
                    limit = 60;
                    break;

                case "day":
                    timeSpan = "day";
                    limit = 1500;
                    break;

                case "week":
                    timeSpan = "week";
                    limit = 168;
                    break;

                case "month":
                    timeSpan = "month";
                    limit = 744;
                    break;
                case "year":
                    timeSpan = "year";
                    limit = 365;
                    break;

                case "all":
                    timeSpan = "all";
                    limit = 0;
                    break;

            }
            UpdatePage();
        }

        private void PinCoin_btn(object sender, RoutedEventArgs e) {
            var c = crypto;
            if (!App.pinnedCoins.Contains(c)) {
                App.pinnedCoins.Add(c);
                Home.AddCoinHome(c);
                FavIcon.Content = "\uEB52";
                inAppNotification.Show(c + " pinned to home.", 2000);
            }
            else {
                Home.RemoveCoinHome(c);
                FavIcon.Content = "\uEB51";
                inAppNotification.Show(c + " unpinned from home.", 2000);
            }
        }

		private async void CompactOverlay_btn_click(object sender, RoutedEventArgs e) {
            var view = ApplicationView.GetForCurrentView();

			var preferences = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
			preferences.CustomSize = new Windows.Foundation.Size(350, 250);

			await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, preferences);
            Frame.Navigate(typeof(CoinCompact), crypto, new SuppressNavigationTransitionInfo());
        }
	}
}
