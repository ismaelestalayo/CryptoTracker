namespace CryptoTracker.Models {
	public class GlobalStats {
        public string TotalMarketCap { get; set; } = "0";
        public string TotalVolume { get; set; } = "0";
        public double BtcDominance { get; set; } = 0;
        public string CurrencySymbol { get; set; }
    }
}
