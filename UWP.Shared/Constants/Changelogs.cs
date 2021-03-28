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
					"Faster launch",
					"Polished UI",
					"Pin any coin as a Live Tile",
					"More granular charts (1h/4h/1d...)",
					"Added automatic refresh of 30secs",
					"Support for Jump Lists",
					"Coins: New Compact Overlay",
					"Portfolio: ability to clone and add notes to purchases" }
				},
			{
				"5.0.0",
				new List<string>() {
					"a", "b"
				}
			}
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
