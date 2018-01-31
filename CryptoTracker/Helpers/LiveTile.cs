using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;

namespace CryptoTracker {
    class LiveTile {

        public void UpdateLiveTile() {

            try {
                LiveTile l = new LiveTile();
                //TO DO: I used to use App.BTC_change1h as the third param
                //SendStockTileNotification("BTC", App.BTC_now, App.BTC_change1h, DateTime.Now);
                //l.SendStockTileNotification("BTC", 0, 0, DateTime.Now);
                //l.SendStockTileNotification("ETH", 0, 0, DateTime.Now);
                //l.SendStockTileNotification("LTC", 0, 0, DateTime.Now);

            } catch (Exception ex) {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        internal void SendStockTileNotification(string symbol, double price, double diff, DateTime dateUpdated) {

            XmlDocument content = GenerateNotificationContent(symbol, price, diff, dateUpdated);

            TileNotification notification = new TileNotification(content);

            notification.Tag = symbol;

            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private XmlDocument GenerateNotificationContent(string symbol, double price, double diff, DateTime dateUpdated) {
            string percentString = (diff < 0 ? "▼" : "▲") + " " + Math.Abs(diff).ToString("N") + "%";

            var content = new TileContent() {
                Visual = new TileVisual() {

                    LockDetailedStatus1 = "BTC: " + "0" + App.coinSymbol,
                    LockDetailedStatus2 = "ETH: " + "0" + App.coinSymbol,
                    LockDetailedStatus3 = "LTC: " + "0" + App.coinSymbol,

                    TileMedium = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText(){
                                    Text = symbol
                                },

                                new AdaptiveText(){
                                    Text = price.ToString("N") + App.coinSymbol
                                },

                                new AdaptiveText(){
                                    Text = percentString,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText(){
                                    Text = dateUpdated.ToString("t"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding() {
                        Content = new TileBindingContentAdaptive() {
                            Children = {
                                new AdaptiveText(){
                                    Text = symbol
                                },

                                new AdaptiveText(){
                                    Text = price.ToString("N") + App.coinSymbol
                                },

                                new AdaptiveText(){
                                    Text = percentString,
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                },

                                new AdaptiveText(){
                                    Text = dateUpdated.ToString("t"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };

            return content.GetXml();
        }


    }
}