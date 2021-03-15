using Microsoft.Toolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Models {
	public class Top100card : ObservableObject{
        private CoinInfo _coinInfo = new CoinInfo();
        public CoinInfo CoinInfo {
            get => _coinInfo;
            set => SetProperty(ref _coinInfo, value);
        }

        private Raw _raw = new Raw();
        public Raw Raw {
            get => _raw;
            set => SetProperty(ref _raw, value);
        }
    }

    public class CoinInfo : ObservableObject {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Algorithm { get; set; }
        public string AssetLaunchDate { get; set; }
        public double MaxSupply { get; set; }
        public int Type { get; set; }
        public string ImageUrl { get; set; } = "/Assets/Icons/iconNULL.png";
        public int Rank { get; set; }
        
        private string _favIcon;
        internal string FavIcon {
            get => _favIcon;
            set => SetProperty(ref _favIcon, value);
        }

        // manually added attributes
        internal string Currency { get; set; } = App.currencySymbol;
        internal string MarketCap { get; set; } = "0";
        internal string Volume { get; set; } = "0";
        internal Brush ChangeFG { get; set; }
    }

    public class Raw {
        public string TYPE { get; set; }
        public string MARKET { get; set; }
        public string FROMSYMBOL { get; set; }
        public string TOSYMBOL { get; set; }
        public double PRICE { get; set; } = 0;
        public double MEDIAN { get; set; } = 0;
        public double VOLUME24HOUR { get; set; } = 0;
        public double VOLUME24HOURTO { get; set; } = 0;

        public double OPEN24HOUR { get; set; } = 0;
        public double HIGH24HOUR { get; set; } = 0;
        public double LOW24HOUR { get; set; } = 0;

        public double VOLUMEHOUR { get; set; } = 0;
        public double VOLUMEHOURTO { get; set; } = 0;
        public double OPENHOUR { get; set; } = 0;
        public double HIGHHOUR { get; set; } = 0;
        public double LOWHOUR { get; set; } = 0;

        public double CHANGE24HOUR { get; set; } = 0;
        public double CHANGEPCT24HOUR { get; set; } = 0;
        public double CHANGEDAY { get; set; } = 0;
        public double CHANGEPCTDAY { get; set; } = 0;
        public double CHANGEHOUR { get; set; } = 0;
        public double CHANGEPCTHOUR { get; set; } = 0;

        public double SUPPLY { get; set; } = 0;
        public double MKTCAP { get; set; } = 0;
        public double TOTALVOLUME24H { get; set; } = 0;
        public double TOTALVOLUME24HTO { get; set; } = 0;
    }
}
