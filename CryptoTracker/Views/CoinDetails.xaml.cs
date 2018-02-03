﻿using CryptoTracker.Helpers;
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

        private static string crypto;
        private static int    limit = 60;
        private static string timeSpan = "day";
        private string Supply = App.stats.Supply;

        public CoinDetails() {
            this.InitializeComponent();

        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {

            String CoinType = e.Parameter.ToString();
            if (CoinType != null) {
                crypto = e.Parameter as string;
            } else {
                crypto = "BTC";
            }

            switch (crypto) {
                case "BTC":
                    cryptoLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/iconBTCc.png"));
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["BTC_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["BTC_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["BTC_colorL"]).Color;

                    Description.Text = "Bitcoin uses peer-to-peer technology to operate with no central authority or banks; managing transactions and the issuing of bitcoins is carried out collectively by "  +
                                       "the network. Bitcoin is open-source; its design is public, nobody owns or controls Bitcoin and everyone can take part.";
                    Website.Text = "bitcoin.org/";
                    Twitter.Text = "#Bitcoin";
                    Reddit.Text = "r/bitcoin";
                    break;

                case "ETH":
                    cryptoLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/iconETHc.png"));
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["ETH_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["ETH_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["ETH_colorL"]).Color;

                    Description.Text = "Ethereum is an open-source, public, blockchain-based distributed computing platform featuring smart contract (scripting) functionality. It provides a decentralized " +
                                       "Turing-complete virtual machine, the Ethereum Virtual Machine (EVM), which can execute scripts using an international network of public nodes. Ethereum also provides" +
                                       " a cryptocurrency token called 'ether', which can be transferred between accounts and used to compensate participant nodes for computations performed.";
                    Website.Text = "ethereum.org/";
                    Twitter.Text = "@ethereumproject";
                    Reddit.Text = "r/ethereum";
                    break;

                case "LTC":
                    cryptoLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/iconLTCc.png"));
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["LTC_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["LTC_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["LTC_colorL"]).Color;

                    Description.Text = "Litecoin is a peer-to-peer Internet currency that enables instant, near-zero cost payments to anyone in the world. Litecoin is an open source, global payment network " +
                                       "that is fully decentralized without any central authorities. Mathematics secures the network and empowers individuals to control their own finances. Litecoin features faster " +
                                       "transaction confirmation times and improved storage efficiency.";
                    Website.Text = "litecoin.org/";
                    Twitter.Text = "@litecoinproject";
                    Reddit.Text = "r/litecoin";
                    break;

                case "XRP":
                    cryptoLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/iconXRPc.png"));
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = ((SolidColorBrush)App.Current.Resources["XRP_color"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = ((SolidColorBrush)App.Current.Resources["XRP_colorT"]).Color;
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = ((SolidColorBrush)App.Current.Resources["XRP_colorL"]).Color;

                    Description.Text = "Also called the Ripple Transaction Protocol (RTXP), it is built upon a distributed open source Internet protocol, consensus ledger and native cryptocurrency called XRP (ripples). " +
                                       "Released in 2012, Ripple purports to enable 'secure, instantly and nearly free global financial transactions of any size with no chargebacks.' At its core, Ripple is based around a" +
                                       " shared, public database or ledger, which uses a consensus process that allows for payments, exchanges and remittance in a distributed process.";
                    Website.Text = "ripple.com/";
                    Twitter.Text = "@ripple";
                    Reddit.Text = "r/ripple";
                    break;

                default:
                    cryptoLogo.Source = new BitmapImage(new Uri("ms-appx:///Assets/iconNULLc.png"));
                    ((SolidColorBrush)App.Current.Resources["coinColor"]).Color  = Windows.UI.Color.FromArgb(255, 43, 42, 42);
                    ((SolidColorBrush)App.Current.Resources["coinColorT"]).Color = Windows.UI.Color.FromArgb(120, 43, 42, 42);
                    ((SolidColorBrush)App.Current.Resources["coinColorL"]).Color = Windows.UI.Color.FromArgb(255, 63, 62, 62);

                    Description.Text = "blip, blop... error getting coin...";
                    Website.Text = "www.google.es";
                    Twitter.Text = "";
                    Reddit.Text = "";
                    break;
            }
            cryptoName.Text = crypto;
            InitValues();
        }

        private void InitValues() {

            TimeSpan period = TimeSpan.FromSeconds(30);
            ThreadPoolTimer PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () => {
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        async private Task UpdateCoin() {
            curr.Text = App.GetCurrentPrice(crypto, "defaultMarket").ToString() + App.coinSymbol;

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
            MarketList.ItemsSource = App.exchanges;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
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
        private void Tapped_Reddit(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            this.Frame.Navigate(typeof(WebVieww), crypto + "_Reddit");
        }
    }
}
