using System.Collections.Generic;
using UWP.Models;

namespace UWP.Core.Constants {
	/// <summary>
	/// Class with key constants for user settings
	/// </summary>
	public class UserSettings {
		public const string AutoRefresh = "AutoRefresh";
		public const string CoinListDate = "CoinListDate";
		public const string Currency = "Currency";
		public const string CurrencySymbol = "CurrencySymbol";
		public const string IsNewUser = "NewUser";
		public const string LastVersion = "LastVersion";
		public const string PinnedCoins = "Pinned";
		public const string Theme = "Theme";

		/// <summary>
		/// Default settings
		/// </summary>
		public static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>(){
			{ AutoRefresh, "1 min" },
			{ CoinListDate,  (double)0 },
			{ Currency, "EUR" },
			{ CurrencySymbol, "€" },
			{ IsNewUser, true },
			{ LastVersion, "0.0.0" },
			{ PinnedCoins, "BTC|ETH|LTC|XRP" },
			{ Theme, "Windows" },
		};
	}

	public class UserStorage {
		public const string Alerts = "Alerts";
		public const string Portfolio = "Portfolio";

	}
}
