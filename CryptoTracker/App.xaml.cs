using CryptoTracker.Helpers;
using CryptoTracker.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CryptoTracker {
    sealed partial class App : Application {
        
        internal static string coin       = "EUR";
        internal static string coinSymbol = "€";

        internal static int pivotIndex = 0;

        internal static List<JSONcoins> coinList = new List<JSONcoins>();
        internal static List<string> pinnedCoins;
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
            public Brush cc { get; set; }
        }

        public App() {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

            try {
                string _theme  = localSettings.Values["Theme"].ToString();
                string _coin   = localSettings.Values["Coin"].ToString();
                string _pinned = localSettings.Values["Pinned"].ToString();
                pinnedCoins = new List<string>(_pinned.Split( new char[] { '|' } ));
                pinnedCoins.Remove("");

                if (_theme != null) {
                    switch (_theme) {
                        case "Light":
                            RequestedTheme = ApplicationTheme.Light;
                            break;
                        case "Dark":
                            RequestedTheme = ApplicationTheme.Dark;
                            break;
                    }
                }
                if (_coin != null) {
                    coin = _coin;
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
                // Light theme and EUR by default and {BTC, ETH, LTC and XRP}
                string err = ex.StackTrace;
                localSettings.Values["Theme"] = "Light";
                localSettings.Values["Coin"] = "EUR";
                localSettings.Values["Pinned"] = "BTC|ETH|LTC|XRP";
                this.RequestedTheme = ApplicationTheme.Light;
                pinnedCoins = new List<string>(new string[] {"BTC", "ETH", "LTC", "XRP"});
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
                JSONcoins.HandleJSON(data);

            } catch (Exception) {
                //var dontWait = await new MessageDialog(ex.Message).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task GetHisto(string crypto, string time, int limit) {
            if (crypto == "MIOTA")
                crypto = "IOT";

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
                JSONhistoric.HandleHistoricJSONnull(crypto, limit);
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
            String URL = "https://krausefx.github.io/crypto-summaries/coins/" + crypto.ToLower() + "-5.txt";
            Uri uri = new Uri(URL);

            try {
                string data = GetStringAsync(uri).Result;
                return data;

            } catch (Exception) {
                return "No description found for this coin.";
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
        internal async static Task<ObservableCollection<Top100coin>> GetTop100() {
            int limit = 50;
            String URL = "https://api.coinmarketcap.com/v1/ticker/?limit=" + limit + "&convert=" + coin;

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            String response = "";

            try {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                ObservableCollection<Top100coin> topCoins = new ObservableCollection<Top100coin>();
                for (int i = 0; i < limit; i++) {
                    if (data[i]["symbol"].ToString() == "MIOTA")
                        data[i]["symbol"] = "IOT";

                    topCoins.Add(
                        new Top100coin {
                            Name            = data[i]["name"].ToString(),
                            Symbol          = data[i]["symbol"].ToString(),
                            Rank            = data[i]["rank"].ToString(),
                            Price           = ToKMB((double)data[i]["price_" + coin.ToLower()]) + coinSymbol,
                            Vol24           = ToKMB((double)data[i]["24h_volume_" + coin.ToLower()]) + coinSymbol,
                            MarketCap       = ToKMB((double)data[i]["market_cap_" + coin.ToLower()]) + coinSymbol,
                            AvailableSupply = data[i]["available_supply"].ToString(),
                            TotalSupply     = data[i]["total_supply"].ToString(),
                            MaxSupply       = data[i]["max_supply"].ToString(),
                            Change1h        = data[i]["percent_change_1h"].ToString() + "%",
                            Change1d        = data[i]["percent_change_24h"].ToString() + "%",
                            Change7d        = data[i]["percent_change_7d"].ToString() + "%",
                            ChangeFG        = data[i]["percent_change_24h"].ToString().StartsWith("-") ? (SolidColorBrush)Current.Resources["pastelRed"] : (SolidColorBrush)Current.Resources["pastelGreen"],
                            Src             = "/Assets/Icons/icon" + data[i]["symbol"].ToString() + ".png",
                            FavIcon         = pinnedCoins.Contains(data[i]["symbol"].ToString()) ? "\uEB52" : "\uEB51"
                        });
                }
                return topCoins;

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.Message).ShowAsync();
                return new ObservableCollection<Top100coin>();
            }
        }
        internal async static Task<GlobalStats> GetGlobalStats() {
            String URL = "https://api.coinmarketcap.com/v1/global/?convert=" + App.coin;

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            String response = "";

            try {
                httpResponse = await httpClient.GetAsync(uri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                GlobalStats g = new GlobalStats();
                g.currency          = coin;
                g.totalMarketCap    = data["total_market_cap_" + coin.ToLower()].ToString();
                g.total24Vol        = data["total_24h_volume_" + coin.ToLower()].ToString();
                g.btcDominance      = data["bitcoin_percentage_of_market_cap"].ToString() + "%";
                g.activeCurrencies  = data["active_currencies"].ToString();
                g.totalMarketCap    = coinSymbol + ToKMB(double.Parse(g.totalMarketCap));
                g.total24Vol        = coinSymbol + ToKMB(double.Parse(g.total24Vol));
                return g;

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.Message).ShowAsync();
                return new GlobalStats();
            }
        }

        public static string ToKMB(double num) {
            if (num > 999999999) {
                return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
            } else
            if (num > 999999) {
                return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
            } else
            if (num > 999) {
                num = Math.Round(num, 2);
                return num.ToString(CultureInfo.InvariantCulture);
            } else {
                num = Math.Round(num, 3);
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// #####
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

