using Microsoft.Toolkit.Uwp.UI.Controls;
using PathConverter;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {
    public sealed partial class Page_CoinTemplate : Page {

        private static string crypto;
        private static int    limit = 60;
        private static string timeSpan = "day";

        public Page_CoinTemplate() {
            this.InitializeComponent();
            InitValues();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

            string CoinType = e.Parameter as string;
            if (CoinType != null) {
                crypto = e.Parameter as string;
            } else {
                crypto = "BTC";
            }

            var stringToPathGeometryConverter = new StringToPathGeometryConverter();
            switch (crypto) {
                case "BTC":
                    CoinName.Text = "Bitcoin";
                    CoinLogoColor.Fill = ((SolidColorBrush)App.Current.Resources["BTC_color"]);
                    CoinLogo.Data = stringToPathGeometryConverter.Convert("M21.78 15.37c.51-.61.83-1.4.83-2.26 0-2.74-1.6-4.38-4.24-4.38V5.45c0-.12-.1-.22-.22-.22h-1.27c-.11 0-.2.1-.2.21v3.3h-1.7V5.44c0-.12-.1-.22-.22-.22H13.5c-.12 0-.2.1-.21.21v3.3H9.67c-.12 0-.21.09-.21.21v1.31c0 .12.1.22.21.22h.21c.94 0 1.7.79 1.7 1.75v7c0 .92-.68 1.67-1.55 1.75a.21.21 0 0 0-.18.16l-.33 1.32c-.01.06 0 .13.04.19.04.05.1.08.17.08h3.55v3.3c0 .1.1.2.2.2h1.28c.12 0 .21-.1.21-.22v-3.28h1.7v3.3c0 .1.1.2.21.2h1.27c.12 0 .22-.1.22-.22v-3.28h.85c2.65 0 4.24-1.64 4.24-4.37 0-1.28-.68-2.39-1.68-3zm-6.8-4.01h2.54c.94 0 1.7.78 1.7 1.75 0 .96-.76 1.75-1.7 1.75h-2.55v-3.5zm3.39 8.75h-3.4v-3.5h3.4c.93 0 1.7.78 1.7 1.75 0 .96-.77 1.75-1.7 1.75z");
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["BTC_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["BTC_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["BTC_colorL"]).Color;
                    break;

                case "ETH":
                    CoinName.Text = "Ethereum";
                    CoinLogoColor.Fill = ((SolidColorBrush)App.Current.Resources["ETH_color"]);
                    CoinLogo.Data = stringToPathGeometryConverter.Convert("M10.13 17.76c-.1-.15-.06-.2.09-.12l5.49 3.09c.15.08.4.08.56 0l5.58-3.08c.16-.08.2-.03.1.11L16.2 25.9c-.1.15-.28.15-.38 0l-5.7-8.13zm.04-2.03a.3.3 0 0 1-.13-.42l5.74-9.2c.1-.15.25-.15.34 0l5.77 9.19c.1.14.05.33-.12.41l-5.5 2.78a.73.73 0 0 1-.6 0l-5.5-2.76z");
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["ETH_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["ETH_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["ETH_colorL"]).Color;
                    break;

                case "LTC":
                    CoinName.Text = "Litecoin";
                    CoinLogoColor.Fill = ((SolidColorBrush)App.Current.Resources["LTC_color"]);
                    CoinLogo.Data = stringToPathGeometryConverter.Convert("M12.29 28.04l1.29-5.52-1.58.67.63-2.85 1.64-.68L16.52 10h5.23l-1.52 7.14 2.09-.74-.58 2.7-2.05.8-.9 4.34h8.1l-.99 3.8z");
                    CoinLogo.Margin = new Thickness(-3);
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["LTC_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["LTC_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["LTC_colorL"]).Color;
                    break;

                case "XRP":
                    CoinName.Text = "Ripple";
                    CoinLogoColor.Fill = ((SolidColorBrush)App.Current.Resources["XRP_color"]);
                    CoinLogo.Data = stringToPathGeometryConverter.Convert("M12.29 28.04l1.29-5.52-1.58.67.63-2.85 1.64-.68L16.52 10h5.23l-1.52 7.14 2.09-.74-.58 2.7-2.05.8-.9 4.34h8.1l-.99 3.8z");
                    CoinLogo.Margin = new Thickness(-3);
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["XRP_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["XRP_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["XRP_colorL"]).Color;
                    break;
            }

        }

        async private void InitValues() {

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
                    RadioButton r = new RadioButton { Content = timeSpan };
                    if (timeSpan == "hour")
                        TimerangeButton_Click(r, null);
                });
            }, period);

            try {
                RadioButton r = new RadioButton { Content = timeSpan };
                TimerangeButton_Click(r, null);

            } catch (Exception) {
                LoadingControl.IsLoading = false;
                curr.Text = "Error!";
                //var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        internal async void UpdatePage() {
            if (LoadingControl == null)
                LoadingControl = new Loading();

            LoadingControl.IsLoading = true;

            await UpdateCoin();
            verticalAxis.Minimum = getMinimum(App.historic);
            verticalAxis.Maximum = getMaximum(App.historic);
            dateTimeAxis = App.AdjustAxis(dateTimeAxis, timeSpan);
            await GetStats();
            await Get24Volume();
        }

        private float getMaximum(List<App.PricePoint> a) {
            int i = 0;
            float max = 0;

            foreach (App.PricePoint type in a) {
                if (a[i].High > max)
                    max = a[i].High;
                i++;
            }
            return max;
        }
        private float getMinimum(List<App.PricePoint> a) {
            int i = 0;
            float min = 15000;

            foreach (App.PricePoint type in a) {
                if (a[i].High < min)
                    min = a[i].High;
                i++;
            }
            return min * (float)0.99;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async private Task UpdateCoin() {
            await App.GetCurrentPrice(crypto);
            switch (crypto) {
                case "BTC":
                    curr.Text = App.BTC_now.ToString() + App.coinSymbol;
                    break;
                case "ETH":
                    curr.Text = App.ETH_now.ToString() + App.coinSymbol;
                    break;
                case "LTC":
                    curr.Text = App.LTC_now.ToString() + App.coinSymbol;
                    break;
                case "XRP":
                    curr.Text = App.XRP_now.ToString() + App.coinSymbol;
                    break;
            }

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
            for (int i = 0; i < limit; ++i) {
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
            switch (crypto) {
                case "BTC":
                    d = (float)Math.Round( ((App.BTC_now / App.BTC_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.BTC_change1h = d;
                    break;
                case "ETH":
                    d = (float)Math.Round(((App.ETH_now / App.ETH_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.ETH_change1h = d;
                    break;
                case "LTC":
                    d = (float)Math.Round(((App.LTC_now / App.LTC_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.LTC_change1h = d;
                    break;
                case "XRP":
                    d = (float)Math.Round(((App.XRP_now / App.XRP_old) - 1) * 100, 2);
                    if (timeSpan.Equals("hour"))
                        App.XRP_change1h = d;
                    break;
            }

            if (d < 0) {
                diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelRed"];
                d = Math.Abs(d);
                diff.Text = "▼" + d.ToString() + "%";
            } else {
                diff.Foreground = (SolidColorBrush)Application.Current.Resources["pastelGreen"];
                diff.Text = "▲" + d.ToString() + "%";
            }

            SplineAreaSeries series = (SplineAreaSeries)priceChart.Series[0];
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "Date" };
            series.ValueBinding = new PropertyNameDataPointBinding() { PropertyName = "Value" };
            series.ItemsSource = data;

            if (LoadingControl != null)
                LoadingControl.IsLoading = false;
        }

        async private Task GetStats() {

            await App.GetStats(crypto);

            statsOpen.Text  = App.stats.Open24   + App.coinSymbol;
            statsHigh.Text  = App.stats.High24   + App.coinSymbol;
            statsLow.Text   = App.stats.Low24    + App.coinSymbol;
            statsVol24.Text = App.stats.Volume24 + crypto;
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
    }
}
