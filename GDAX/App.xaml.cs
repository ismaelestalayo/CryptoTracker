using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace GDAX {
    sealed partial class App : Application {

        internal static float currency_BTC;
        internal static float currency_ETH;
        internal static float currency_LTC;
        internal static float USD_EUR;
        internal static bool EUR = true;
        internal static bool firstTime = true;
        
        internal static String ss { get; set; }

        internal static List<PricePoint> pp = new List<PricePoint>();

        static HttpClient client = new HttpClient();

        class Ssttringg {
            private String GetString() {
                return ss;
            }
        }

        public App() {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Se invoca cuando el usuario final inicia la aplicación normalmente. Se usarán otros puntos
        /// de entrada cuando la aplicación se inicie para abrir un archivo específico, por ejemplo.
        /// </summary>
        /// <param name="e">Información detallada acerca de la solicitud y el proceso de inicio.</param>
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


        async internal static Task GetData(string currency_pair) {

            String URL = "https://api.coinbase.com/v2/prices/" + currency_pair + "/spot";
            String response;
            try {
                response = await client.GetStringAsync(URL);
            } catch {
                response = " ";
            }
            var data = JObject.Parse(response);

            if (currency_pair == "BTC-EUR")
                currency_BTC = (float)data["data"]["amount"];
            else if (currency_pair == "ETH-EUR")
                currency_ETH = (float)data["data"]["amount"];
            else if (currency_pair == "LTC-EUR")
                currency_LTC = (float)data["data"]["amount"];

            URL = "https://api.coinbase.com/v2/exchange-rates";
            try {
                response = await client.GetStringAsync(URL);
            }
            catch {
                response = " ";
            }
            data = JObject.Parse(response);
            USD_EUR = (float)data["data"]["rates"]["EUR"];
        }

        async internal static Task GetHistoricValues(int granularity, string currency_pair) {
            String URL = "https://api.gdax.com/products/" + currency_pair + "/candles";

            if (granularity != 0)
                URL += "?granularity=" + granularity; //https://api.gdax.com/products/ETH-EUR/candles?granularity=60

            HttpClient httpClient = new HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header)) {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header)) {
                throw new Exception("Invalid header value: " + header);
            }

            Uri requestUri = new Uri(URL);

            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string response = "";

            try {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();

                response = await httpResponse.Content.ReadAsStringAsync();
                var data = JRaw.Parse(response);

                int count = ((JContainer)data).Count;

                pp.Clear();
                //List<PricePoint> p = new List<PricePoint>();
                for (int i = 0; i < count; i++) {
                    pp.Add( PricePoint.GetPricePoint(data[i], currency_pair) );
                }
                


            }
            catch (Exception ex) {
                response = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        public class PricePoint {
            public int LinuxTime;
            public string Date { get; set; }
            public string Currency { get; set; }
            public DateTime DateTime { get; set; }
            public float Low { get; set; }
            public float High { get; set; }
            public float Open { get; set; }
            public float Close { get; set; }
            public float Volume { get; set; }

            public static PricePoint GetPricePoint(JToken test, string currency_pair) {
                PricePoint p = new PricePoint();

                p.Currency = currency_pair;

                p.LinuxTime = (int)test[0];
                p.Low = (float)test[1];
                p.High = (float)test[2];
                p.Open = (float)test[3];
                p.Close = (float)test[4];
                p.Volume = (float)test[5];

                if (currency_pair.EndsWith("USD"))
                    p.Low = p.Low * USD_EUR;

                int unixTimeStamp = p.LinuxTime;

                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                DateTime date = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                p.DateTime = date;
                p.Date = date.ToString();

                return p;
            }
        }




    }
}
