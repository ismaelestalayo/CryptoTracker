using Microsoft.Toolkit.Mvvm.ComponentModel;

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
        public string FavIcon {
            get => _favIcon;
            set => SetProperty(ref _favIcon, value);
        }
    }

    public class Raw {
        public string TYPE { get; set; }
        public string MARKET { get; set; }
        public string FROMSYMBOL { get; set; }
        public string TOSYMBOL { get; set; }
        public double PRICE { get; set; }
        public double MEDIAN { get; set; }
        public double VOLUME24HOUR { get; set; }
        public double VOLUME24HOURTO { get; set; }

        public double OPEN24HOUR { get; set; }
        public double HIGH24HOUR { get; set; }
        public double LOW24HOUR { get; set; }

        public double VOLUMEHOUR { get; set; }
        public double VOLUMEHOURTO { get; set; }
        public double OPENHOUR { get; set; }
        public double HIGHHOUR { get; set; }
        public double LOWHOUR { get; set; }

        public double CHANGE24HOUR { get; set; }
        public double CHANGEPCT24HOUR { get; set; }
        public double CHANGEDAY { get; set; }
        public double CHANGEPCTDAY { get; set; }
        public double CHANGEHOUR { get; set; }
        public double CHANGEPCTHOUR { get; set; }

        public double SUPPLY { get; set; }
        public double MKTCAP { get; set; }
        public double TOTALVOLUME24H { get; set; }
        public double TOTALVOLUME24HTO { get; set; }
    }
}
