using System.Collections.Generic;

namespace CryptoTracker.Core.Constants {
	/// <summary>
	/// Class with key constants for user settings
	/// </summary>
	public class UserSettingsConstants {
		public const string Theme = "Theme";
		public const string Currency = "Currency";
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
			{ PinnedCoins, "BTC|ETH|LTC|XRP" },
			{ CoinListDate,  (double)0 },
			{ IsNewUser, true },
			{ LastVersion, "0.0.0" }
		};
	}
}
