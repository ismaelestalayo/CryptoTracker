using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CoinBase {
    sealed partial class App : Application {

        internal static float USD_EUR;
        internal static string coin = "EUR";

        internal static float BTC_old;
        internal static float ETH_old;
        internal static float LTC_old;

        internal static float BTC_now;
        internal static float ETH_now;
        internal static float LTC_now;

        internal static List<PricePoint> ppBTC = new List<PricePoint>();
        internal static List<PricePoint> ppETH = new List<PricePoint>();
        internal static List<PricePoint> ppLTC = new List<PricePoint>();
        internal static PricePoint stats = new PricePoint();

        static HttpClient client = new HttpClient();

        internal static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public App() {

            string x, y;

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
                
            } catch (Exception ex) {
                // Light theme and EUR by default (first time on the app)
                localSettings.Values["Theme"] = "Light";
                localSettings.Values["Coin"] = "EUR";
                this.RequestedTheme = ApplicationTheme.Light;
            }


            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;

            // No repetir la inicialización de la aplicación si la ventana tiene contenido todavía,
            // solo asegurarse de que la ventana está activa.
            if (rootFrame == null) {
                // Crear un marco para que actúe como contexto de navegación y navegar a la primera página.
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
                    // Cuando no se restaura la pila de navegación, navegar a la primera página,
                    // configurando la nueva página pasándole la información requerida como
                    //parámetro de navegación
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Asegurarse de que la ventana actual está activa.
                Window.Current.Activate();
            }
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e) {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e) {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Guardar el estado de la aplicación y detener toda actividad en segundo plano
            deferral.Complete();
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////
        async internal static Task GetData(string crypto) {

            String URL = "https://api.coinbase.com/v2/prices/" +crypto+ "-" +coin + "/spot";
            String response;
            try {
                response = await client.GetStringAsync(URL);
            } catch {
                response = " ";
            }
            var data = JObject.Parse(response);

            switch (crypto) {
                case "BTC":
                    BTC_now = (float)data["data"]["amount"];
                    break;

                case "ETH":
                    ETH_now = (float)data["data"]["amount"];
                    break;

                case "LTC":
                    LTC_now = (float)data["data"]["amount"];
                    break;
            }

            URL = "https://api.coinbase.com/v2/exchange-rates";
            try {
                response = await client.GetStringAsync(URL);
            } catch {
                response = " ";
            }
            data = JObject.Parse(response);
            USD_EUR = (float)data["data"]["rates"]["EUR"];
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        async internal static Task GetHisto(string crypto, string time, int limit) {
            String URL = "https://min-api.cryptocompare.com/data/histo" + time + "?e=CCCAGG&fsym="
                + crypto + "&tsym=" + coin + "&limit=" + limit;

            if (limit == 0)
                URL = "https://min-api.cryptocompare.com/data/histoday?e=CCCAGG&fsym=" + crypto + "&tsym=" + coin + "&allData=true";

            Uri requestUri = new Uri(URL);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;
            String header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header)) {
                throw new Exception("Invalid header value: " + header);
            }

            string response = "";

            try {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JRaw.Parse(response);

                if (crypto.Equals("BTC")) {
                    ppBTC.Clear();

                    for (int i = 0; i < limit; i++) {
                        ppBTC.Add(PricePoint.GetPricePoint(data["Data"][i], coin));
                    }
                    BTC_now = (float)Math.Round((float)data["Data"][limit]["close"], 2);
                    BTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);

                } else if (crypto.Equals("ETH")) {
                    ppETH.Clear();

                    for (int i = 0; i < limit; i++) {
                        ppETH.Add(PricePoint.GetPricePoint(data["Data"][i], coin));
                    }
                    ETH_now = (float)Math.Round( (float)data["Data"][limit]["close"], 2);
                    ETH_old = (float)Math.Round( (float)data["Data"][0]["close"], 2);

                } else if (crypto.Equals("LTC")) {
                    ppLTC.Clear();

                    for (int i = 0; i < limit; i++) {
                        ppLTC.Add(PricePoint.GetPricePoint(data["Data"][i], coin));
                    }
                    LTC_now = (float)Math.Round((float)data["Data"][limit]["close"], 2);
                    LTC_old = (float)Math.Round((float)data["Data"][0]["close"], 2);
                }

            } catch (Exception ex) {
                response = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
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

            public static PricePoint GetPricePoint(JToken test, string currency_pair) {
                PricePoint p = new PricePoint();

                p.LinuxTime = (int)test["time"];
                p.Low = (float)test["low"];
                p.High = (float)test["high"];
                p.Open = (float)test["open"];
                p.Close = (float)test["close"];
                p.Volumefrom = (float)test["volumefrom"];
                p.Volumeto = (float)test["volumeto"];

                int unixTimeStamp = p.LinuxTime;

                DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                p.DateTime = date;
                p.Date = date.ToString();

                return p;
            }
        }
    }



}

