using System.Collections.Generic;

namespace UWP.Core.Constants {
    /// <summary>
	/// Class with the changelogs of the latest versions
	/// </summary>
    public class Changelogs {

		public static readonly Dictionary<string, List<string>> LatestChangelogs = new Dictionary<string, List<string>>(){
			{
				"6.0.0",
				new List<string>(){
					"Refreshed UI inline with Windows 11",
					"New volume and candle charts",
					"New price alerts",
					"Import/Export portfolio backups",
					"More granular time filters (1h/4h/1d...)",
					"Pin any coin as a Live Tile (Windows 10)",
					"Configurable auto-refresh and startup page",
					"Support for Jump Lists",
					"New Compact Overlay",
					"Portfolio shows diversification ratios and can group entries",
					"Ability to duplicate, add notes and change the currency of purchases"
				}
			},
			//{
			//	"5.0.0",
			//	new List<string>() {
			//		"a", "b"
			//	}
			//}
		};

		public static readonly List<string> CurrentChangelog = LatestChangelogs["6.0.0"];

		public static string FormatChangelog(List<string> changelog) {
			string message = "";
			foreach (var change in changelog)
				message += $"  • {change} \n";
			return message;
		}
	}
}
