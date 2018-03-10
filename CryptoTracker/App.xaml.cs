﻿using CryptoTracker.Helpers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {
    sealed partial class App : Application {
        
        internal static string coin       = "EUR";
        internal static string coinSymbol = "€";

        internal static int pivotIndex = 0;

        internal static List<string> coinList = new List<string>();
        internal static List<string> pinnedCoins = new List<string>(new string[] { "BTC", "ETH", "ADA", "BCH", "XRP", "LTC"});
        internal static List<JSONhistoric> historic = new List<JSONhistoric>();
        internal static JSONstats stats = new JSONstats();
        internal static List<JSONexchanges> exchanges = new List<JSONexchanges>();

        internal static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        internal class ChartDataObject {
            public DateTime Date { get; set; }
            public float Value { get; set; }
            public float Low { get; set; }
            public float High { get; set; }
            public float Open { get; set; }
            public float Close { get; set; }
            public float Volume { get; set; }
            public string Category { get; set; }
        }

        public App() {

            string x, y;

            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            try {
                x = localSettings.Values["Theme"].ToString();
                y = localSettings.Values["Coin"].ToString();

                if (x != null) {
                    switch (x) {
                        case "Light":
                            RequestedTheme = ApplicationTheme.Light;
                            break;
                        case "Dark":
                            RequestedTheme = ApplicationTheme.Dark;
                            break;
                    }
                }
                if (y != null) {
                    coin = y;
                    switch (coin) {
                        case "EUR":
                            coinSymbol = "€";
                            break;
                        case "GBP":
                            coinSymbol = "£";
                            break;
                        case "USD":
                        case "CAD":
                        case "AUD":
                        case "MXN":
                            coinSymbol = "$";
                            break;
                        case "CNY":
                        case "JPY":
                            coinSymbol = "¥";
                            break;
                        case "INR":
                            coinSymbol = "₹";
                            break;
                    }
                }
                
            } catch (Exception ex){
                // Light theme and EUR by default (first time on the app)
                string err = ex.StackTrace;
                localSettings.Values["Theme"] = "Light";
                localSettings.Values["Coin"] = "EUR";
                this.RequestedTheme = ApplicationTheme.Light;
                pinnedCoins = new List<string>(new string[] { "BTC", "ETH", "LTC" });
            }

            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null) {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Cargar el estado de la aplicación suspendida previamente
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(900, 550));
                Window.Current.Activate();
            }

            AppCenter.Start("37e61258-8639-47d6-9f6a-d47d54cd8ad5", typeof(Analytics));
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal static double GetCurrentPrice(string crypto, string market) {
            string URL = "https://min-api.cryptocompare.com/data/price?fsym=" +crypto+ "&tsyms=" + App.coin;

            if (market != "defaultMarket") 
                URL += "&e=" + market;

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();

            try {
                var data = GetJSONAsync(uri).Result;
                return Math.Round((float)data[coin], 2);
                
            } catch (Exception) {
                return 0;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task GetCoinList() {
            String URL = "https://min-api.cryptocompare.com/data/top/totalvol?limit=100&tsym=" + coin;
            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();

            try {
                var data = await GetJSONAsync(uri);

                for (int i = 0; i < 100; i++) {
                    coinList.Add( data["Data"][i]["CoinInfo"]["Name"].ToString() );
                }

            } catch (Exception) {
                //var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task GetHisto(string crypto, string time, int limit) {
            //CCCAGG Bitstamp Bitfinex Coinbase HitBTC Kraken Poloniex 
            string URL = "https://min-api.cryptocompare.com/data/histo" + time + "?e=CCCAGG&fsym="
                + crypto + "&tsym=" + coin + "&limit=" + limit;

            if (limit == 0)
                URL = "https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym=" + crypto + "&tsym=" + coin + "&allData=true";

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();

            try {
                string response = await httpClient.GetStringAsync(uri);
                var data = JToken.Parse(response);

                JSONhistoric.HandleHistoricJSON(data, crypto);                
                

            } catch (Exception) {
                //var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task GetCoinStats(string crypto, string market) {

            string URL = "https://min-api.cryptocompare.com/data/pricemultifull?fsyms=" + crypto + "&tsyms=" + coin;
            
            if(market != "defaultMarket") 
                URL += "&e=" + market;

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();

            try {
                string response = await httpClient.GetStringAsync(uri);
                var data = JToken.Parse(response);

                stats = JSONstats.HandleStatsJSON(data, crypto, coin);

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task GetTopExchanges(string crypto, string toSym) {

            String URL = "https://min-api.cryptocompare.com/data/top/exchanges?fsym=" + crypto  +"&tsym="+ toSym + "&limit=8";

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            String response = "";

            try {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                exchanges.Clear();
                int lastIndex = ((JContainer)data["Data"]).Count;
                for (int i = 0; i < lastIndex; i++) {
                    exchanges.Add(JSONexchanges.GetExchanges(data["Data"][i]));
                }

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal static string GetCoinDescription(string crypto) {
            String URL = "https://krausefx.github.io/crypto-summaries/coins/" +crypto.ToLower()+ "-5.txt";
            Uri uri = new Uri(URL);

            try {
                string data = GetStringAsync(uri).Result;
                

                return data;

            } catch (Exception) {
                return "No description found for this coin.";
            }
        }



        /// <summary>
        /// do NOT mess with async methods...
        /// 
        /// Thank god I found this article (thanks Stephen Cleary)
        /// http://blog.stephencleary.com/2012/07/dont-block-on-async-code.html
        /// 
        /// </summary>
        private static async Task<JToken> GetJSONAsync(Uri uri) {

            using (var client = new HttpClient()) {
                var jsonString = await client.GetStringAsync(uri).ConfigureAwait(false);
                return JToken.Parse(jsonString);
            }
        }
        private static async Task<string> GetStringAsync(Uri uri) {
            using (var client = new HttpClient()) {
                var s = await client.GetStringAsync(uri).ConfigureAwait(false);
                return s;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal static DateTimeContinuousAxis AdjustAxis(DateTimeContinuousAxis DateTimeAxis, string timeSpan) {
            switch (timeSpan) {
                case "hour":
                    DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    DateTimeAxis.MajorStep = 10;
                    DateTimeAxis.Minimum = DateTime.Now.AddHours(-1);
                    break;

                case "day":
                    DateTimeAxis.LabelFormat = "{0:HH:mm}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    DateTimeAxis.MajorStep = 6;
                    DateTimeAxis.Minimum = DateTime.Now.AddDays(-1);
                    break;

                case "week":
                    DateTimeAxis.LabelFormat = "{0:ddd d}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    DateTimeAxis.MajorStep = 1;
                    DateTimeAxis.Minimum = DateTime.Now.AddDays(-7);
                    break;

                case "month":
                    DateTimeAxis.LabelFormat = "{0:d/M}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    DateTimeAxis.MajorStep = 1;
                    DateTimeAxis.Minimum = DateTime.Now.AddMonths(-1);
                    break;
                case "year":
                    DateTimeAxis.LabelFormat = "{0:MMM}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    DateTimeAxis.MajorStep = 1;
                    DateTimeAxis.Minimum = DateTime.MinValue;
                    break;

                case "all":
                    DateTimeAxis.LabelFormat = "{0:MMM/yy}";
                    DateTimeAxis.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    DateTimeAxis.MajorStep = 4;
                    DateTimeAxis.Minimum = DateTime.MinValue;
                    break;
            }
            return DateTimeAxis;
        }



    }

}
