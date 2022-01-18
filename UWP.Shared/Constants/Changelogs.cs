using System.Collections.Generic;
using System.Linq;

namespace UWP.Core.Constants {
    /// <summary>
	/// Class with the changelogs of the latest versions
	/// </summary>
    public class Changelogs {

		public static readonly Dictionary<string, List<string>> LatestChangelogs = new Dictionary<string, List<string>>(){
			{
				"6.1",
				new List<string>() {
					"General: added CZK currency",
					"General: fixed auto-refresh",
					"Settings: set the chart's default timespan and optional Back button",
					"Home: adaptive layout",
					"News: removed AmbCrypto (it crashed)",
					"Portfolio: fixed rounding errors with ROI",
				}
			},
			{
				"6.0.4",
				new List<string>() {
					"General: fixed casting error on startup",
					"Coins: SHIB and other alt-coins' empty graphs fixed",
					"Portfolio: fixed rounding errors",
				}
			},
			{
				"6.0.3",
				new List<string>(){
					"Settings: set the currency to all purchases",
					"Portfolio: group button shown only when multiple same coin entries exist",
					"Portfolio: fixed error saving the Portfolio",
				}
			},
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
					"Portfolio: diversification ratios, group or duplicate entries, add notes and change purchases' currencies"
				}
			}
		};

		public static readonly List<string> CurrentChangelog = LatestChangelogs[LatestChangelogs.Keys.ToList()[0]];
		public static readonly List<string> MajorChangelog = LatestChangelogs["6.0.0"];

		public static string FormatChangelog(List<string> changelog) {
			string message = "";
			foreach (var change in changelog)
				message += $"  • {change} \n";
			return message.TrimEnd('\n');
		}
	}
}
