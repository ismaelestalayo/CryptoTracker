using System.Collections.Generic;
using UWP.Models;

namespace UWP.Core.Constants {
	/// <summary>
	/// Class with key constants for user settings
	/// </summary>
	public class UserSettings {
		public const string Theme = "Theme";
		public const string Currency = "Currency";
		public const string CurrencySymbol = "CurrencySymbol";
		public const string PinnedCoins = "Pinned";
		public const string IsNewUser = "NewUser";
		public const string CoinListDate = "CoinListDate";
		public const string LastVersion = "LastVersion";

		/// <summary>
		/// Default settings
		/// </summary>
		public static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>(){
			{ Theme, "Windows" },
			{ Currency, "EUR" },
			{ CurrencySymbol, "€" },
			{ PinnedCoins, "BTC|ETH|LTC|XRP" },
			{ CoinListDate,  (double)0 },
			{ IsNewUser, true },
			{ LastVersion, "0.0.0" },
		};
	}

	public class UserStorage {
		public const string Alerts = "Alerts";
		public const string Portfolio = "Portfolio";

	}
}
