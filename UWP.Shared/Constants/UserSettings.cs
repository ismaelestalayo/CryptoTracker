using System.Collections.Generic;

namespace UWP.Core.Constants {
    /// <summary>
    /// Class with key constants for user settings
    /// </summary>
    public class UserSettings {
		public const string AutoRefresh = "AutoRefresh";
		public const string ChartShowAlerts = "ChartShowAlerts";
		public const string ChartShowPurchases = "ChartShowPurchases";
		public const string CoinListsDate = "CoinListsDate";
		public const string Currency = "Currency";
		public const string CurrencySymbol = "CurrencySymbol";
		public const string IsBackButtonVisible = "IsBackButtonVisible";
		public const string IsNewUser = "NewUser";
		public const string LastVersion = "LastVersion";
		public const string Minimal = "Minimal";
		public const string Monochrome = "Monochrome";
		public const string BW = "BlackAndWhite";
		public const string PinnedCoins = "Pinned";
		public const string StartupPage = "StartupPage";
		public const string Theme = "Theme";
		public const string Timespan = "Timespan";

		/// <summary>
		/// Default settings
		/// </summary>
		public static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>(){
			{ AutoRefresh, "1 min" },
			{ BW, false },
			{ ChartShowAlerts, true },
			{ ChartShowPurchases, true },
			{ CoinListsDate,  (double)0 },
			{ Currency, "EUR" },
			{ CurrencySymbol, "€" },
			{ IsBackButtonVisible, true },
			{ IsNewUser, true },
			{ LastVersion, "0.0.0" },
			{ Minimal, true },
			{ Monochrome, false },
			{ PinnedCoins, "BTC|ETH|LTC|XRP|ADA|AVAX" },
			{ StartupPage, "Home" },
			{ Theme, "Windows" },
			{ Timespan, "1w" },
		};
	}

	public class UserStorage {
		/// <summary>
		/// List<Alert>
		/// </summary>
		public const string Alerts = "Alerts";

		/// <summary>
		/// List<CoinGeckoCoin>
		/// </summary>
		public const string CacheCoinGecko = "CacheCoinGecko";

		/// <summary>
		/// List<CoinPaprikaCoin>
		/// </summary>
		public const string CacheCoinPaprika = "CacheCoinPaprika";

		/// <summary>
		/// List<PurchaseModel>
		/// </summary>
		public const string Portfolio6 = "Portfolio6";

		/// <summary>
		/// List<CoinMarket>
		/// </summary>
		public const string CoinsCache = "CoinsCache";
	}
}
