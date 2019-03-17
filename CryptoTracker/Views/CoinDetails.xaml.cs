using CryptoTracker.Helpers;
using CryptoTracker.Views;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {
    public sealed partial class CoinDetails : Page {

        internal static string crypto { get; set; }
        private static int    limit = 60;
        private static string timeSpan = "hour";
        private string Supply = App.stats.Supply;
        JSONcoins coin;

        public CoinDetails() {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            
            if (e.Parameter.ToString() != null) {
                crypto = e.Parameter as string;
            } else {
                crypto = "NULL";
            }

            Frame contentFrame = Window.Current.Content as Frame;
            MainPage mp = contentFrame.Content as MainPage;
            TextBlock title = mp.FindName("mainTitle") as TextBlock;
            Image titleLogo = mp.FindName("mainTitleLogo") as Image;
            title.Text      = App.coinList.Find(x => x.Name == crypto).FullName + " (" + crypto + ")";

            try {
                titleLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/icon" + crypto.ToUpper() + ".png"));
            } catch(Exception) {
                titleLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/iconNULL.png"));
            }
            titleLogo.Visibility = Visibility.Visible;

            try {
                ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources[crypto.ToUpper() + "_color"]).Color;
                ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources[crypto.ToUpper() + "_colorT"]).Color;
            } catch {
                ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["null_colorT"]).Color;
                ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["null_colorT"]).Color;
            }

            InitValues();
        }

        private async void InitValues() {

            JSONcoins coin = App.coinList.Find(x => x.Name == crypto);
            JSONsnapshot snapshot = await App.GetCoinInfo(coin.Id);
            //Description.Text = snapshot.Description;
            Description.Text = App.GetCoinDescription(crypto);
            Website.Text = snapshot.WebSiteURL;
            Twitter.Text = snapshot.Twitter;

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
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
            verticalAxis.Minimum = GetMinimum(App.historic);
            verticalAxis.Maximum = GetMaximum(App.historic);
            dateTimeAxis = App.AdjustAxis(dateTimeAxis, timeSpan);
            await GetStats();
            await Get24Volume();
            await GetExchanges();
        }

        private float GetMaximum(List<JSONhistoric> a) {
            int i = 0;
            float max = 0;

            foreach (JSONhistoric type in a) {
                if (a[i].High > max)
                    max = a[i].High;
                i++;
            }
            return max;
        }
        private float GetMinimum(List<JSONhistoric> a) {
            int i = 0;
            float min = 15000;

            foreach (JSONhistoric type in a) {
                if (a[i].High < min)
                    min = a[i].High;
                i++;
            }
            return min * (float)0.99;
        }
        // #########################################################################################
        async private Task UpdateCoin() {

            Frame contentFrame = Window.Current.Content as Frame;
            MainPage mp = contentFrame.Content as MainPage;
            TextBlock titleVal = mp.FindName("mainTitleVal") as TextBlock;
            TextBlock titleDiff = mp.FindName("mainTitleDiff") as TextBlock;
            titleVal.Text = App.GetCurrentPrice(crypto, "defaultMarket").ToString() + App.coinSymbol;
            titleVal.Visibility  = Visibility.Visible;
            titleDiff.Visibility = Visibility.Visible;

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

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < App.historic.Count; ++i) {
                App.ChartDataObject obj = new App.ChartDataObject {
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

            float d = 0;
            float oldestPrice = App.historic[0].Close;
            float newestPrice = App.historic[App.historic.Count - 1].Close;
            d = (float)Math.Round( ((newestPrice / oldestPrice) - 1) * 100, 2);

            if (d < 0) {
                titleDiff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                d = Math.Abs(d);
                titleDiff.Text = "▼" + d.ToString() + "%";
            } else {
                titleDiff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                titleDiff.Text = "▲" + d.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)priceChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        // copied to Home.xaml.cs
        async private Task GetStats() {

            await App.GetCoinStats(crypto, "defaultMarket");

            statsOpen.Text  = App.stats.Open24;
            statsHigh.Text  = App.stats.High24;
            statsLow.Text   = App.stats.Low24;
            statsVol24.Text = App.stats.Volume24;
            supply.Text     = App.stats.Supply;
            marketcap.Text  = App.stats.Marketcap;
            totVol24.Text   = "Total Vol 24h: " + App.stats.Volume24;
            totVol24to.Text = "Total Vol 24h to: " + App.stats.Volume24To;
        }
        async private Task Get24Volume() {
            await App.GetHisto(crypto, "hour", 24);

            List<App.ChartDataObject> data = new List<App.ChartDataObject>();
            for (int i = 0; i < 24; i++) {
                data.Add(new App.ChartDataObject() {
                    Date   = App.historic[i].DateTime,
                    Volume = App.historic[i].Volumefrom
                });
            }
            this.volumeChart.DataContext = data;
        }
        async private Task GetExchanges() {
            await App.GetTopExchanges(crypto, App.coin);

            if (App.exchanges.Count != 0) {
                noMarketsWarning.Visibility = Visibility.Collapsed;
                MarketList.ItemsSource = App.exchanges;
            } else {
                noMarketsWarning.Visibility = Visibility.Visible;
            }
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

        private void Tapped_Website(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(WebVieww), crypto + "_Website");
        }
        private void Tapped_Twitter(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(WebVieww), crypto + "_Twitter");
        }
    }
}
