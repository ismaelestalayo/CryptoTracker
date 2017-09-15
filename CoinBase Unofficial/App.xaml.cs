using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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

namespace CoinBase {
    sealed partial class App : Application {
        
        internal static string coin = "EUR";

        internal static float BTC_old;
        internal static float ETH_old;
        internal static float LTC_old;

        internal static float BTC_now;
        internal static float ETH_now;
        internal static float LTC_now;

        internal static float BTC_change1h = 0;
        internal static float ETH_change1h = 0;
        internal static float LTC_change1h = 0;

        internal static List<PricePoint> ppBTC = new List<PricePoint>();
        internal static List<PricePoint> ppETH = new List<PricePoint>();
        internal static List<PricePoint> ppLTC = new List<PricePoint>();
        internal static PricePoint stats = new PricePoint();

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
                            this.RequestedTheme = ApplicationTheme.Light;
                            break;
                        case "Dark":
                            this.RequestedTheme = ApplicationTheme.Dark;
                            break;
                    }
                }
                if (y != null)
                    coin = y;
                
            } catch (Exception ex){
                // Light theme and EUR by default (first time on the app)
                string err = ex.StackTrace;
                localSettings.Values["Theme"] = "Light";
                localSettings.Values["Coin"] = "EUR";
                this.RequestedTheme = ApplicationTheme.Light;
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

                // Poner el marco en la ventana actual.
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false) {
                if (rootFrame.Content == null) {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(900, 550));
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        async internal static Task GetCurrentPrice(string crypto) {
            
            var uri = new Uri("https://min-api.cryptocompare.com/data/price?fsym=" + crypto + "&tsyms=EUR,USD,CAD,MXN");
            HttpClient httpClient = new HttpClient();
            String response = "";

            try {
                //Send the GET request
                response = await httpClient.GetStringAsync(uri);
                var data = JToken.Parse(response);

                switch (crypto) {
                    case "BTC":
                        BTC_now = (float)data[coin];
                        break;

                    case "ETH":
                        ETH_now = (float)data[coin];
                        break;

                    case "LTC":
                        LTC_now = (float)data[coin];
                        break;
                }

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        internal async static Task GetHisto(string crypto, string time, int limit) {
            String URL = "https://min-api.cryptocompare.com/data/histo" + time + "?e=CCCAGG&fsym="
                + crypto + "&tsym=" + coin + "&limit=" + limit;

            if (limit == 0)
                URL = "https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym=" + crypto + "&tsym=" + coin + "&allData=true";

            Uri uri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            string response = "";

            try {
                //Send the GET request
                response = await httpClient.GetStringAsync(uri);
                var data = JToken.Parse(response);

                switch (crypto) {
                    case "BTC":
                        ppBTC.Clear();

                        for (int i = 0; i < limit; i++) {
                            ppBTC.Add(PricePoint.GetPricePointHisto(data["Data"][i]));
                        }
                        BTC_now = (float)Math.Round((float)data["Data"][limit]["close"], 2);
                        BTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                        break;


                    case "ETH":
                        ppETH.Clear();

                        for (int i = 0; i < limit; i++) {
                            ppETH.Add(PricePoint.GetPricePointHisto(data["Data"][i]));
                        }
                        ETH_now = (float)Math.Round((float)data["Data"][limit]["close"], 2);
                        ETH_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                        break;

                    case "LTC":
                        ppLTC.Clear();

                        for (int i = 0; i < limit; i++) {
                            ppLTC.Add(PricePoint.GetPricePointHisto(data["Data"][i]));
                        }
                        LTC_now = (float)Math.Round((float)data["Data"][limit]["close"], 2);
                        LTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                        break;
                }

            } catch (Exception ex) {
                var dontWait = await new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        internal async static Task GetStats(string crypto) {

            String URL = "https://www.cryptocompare.com/api/data/coinsnapshot/?fsym=" +crypto+ "&tsym=" + coin;

            Uri requestUri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            string response = "";

            try {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JToken.Parse(response);

                stats = PricePoint.GetPricePointStats(data["Data"]["AggregatedData"]);

            } catch (Exception ex) {
                response = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
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

        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        public class PricePoint {
            public int LinuxTime;
            public string Date { get; set; }
            public DateTime DateTime { get; set; }
            public float Low { get; set; }
            public float High { get; set; }
            public float Open { get; set; }
            public float Close { get; set; }
            public float Volumefrom { get; set; }
            public float Volumeto { get; set; }

            // For stats only
            public float Low24 { get; set; }
            public float High24 { get; set; }
            public float Open24 { get; set; }
            public float Volume24 { get; set; }
            public float Volume24To { get; set; }

            public static PricePoint GetPricePointHisto(JToken data) {
                PricePoint p = new PricePoint();

                p.LinuxTime  = (int)data["time"];
                p.Low        = (float)Math.Round((float)data["low"], 2);
                p.High       = (float)Math.Round((float)data["high"], 2);
                p.Open       = (float)Math.Round((float)data["open"], 2);
                p.Close      = (float)Math.Round((float)data["close"], 2);
                p.Volumefrom = (float)Math.Round((float)data["volumefrom"], 2);
                p.Volumeto   = (float)Math.Round((float)data["volumeto"], 2);

                int unixTimeStamp = p.LinuxTime;

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                p.DateTime = date;
                p.Date = date.ToString();

                return p;
            }

            public static PricePoint GetPricePointStats(JToken data) {
                PricePoint p = new PricePoint();

                p.Low24 = (float)Math.Round((float)data["LOW24HOUR"], 2);
                p.High24 = (float)Math.Round((float)data["HIGH24HOUR"], 2);
                p.Open24 = (float)Math.Round((float)data["OPEN24HOUR"], 2);
                p.Volume24 = (float)Math.Round((float)data["VOLUME24HOUR"], 2);
                p.Volume24To = (float)Math.Round((float)data["VOLUME24HOURTO"], 2);

                return p;
            }
        }



    }

}

