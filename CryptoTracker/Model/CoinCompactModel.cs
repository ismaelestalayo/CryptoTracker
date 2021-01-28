using CryptoTracker.Helpers;
using System.Collections.Generic;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Model {
	internal class CoinCompactModel {
		internal string Crypto { get; set; }
		internal string Currency { get; set; }
		internal double CurrentPrice { get; set; }
		internal double CurrentDiff { get; set; }
		internal string CurrentDiffArrow { get; set; }
		internal Brush DiffFG { get; set; }
		internal Brush ChartStroke { get; set; }
		internal Color ChartFill1 { get; set; }
		internal Color ChartFill2 { get; set; }
		internal float HistoricMin { get; set; } = 20000;
		internal float HistoricMax { get; set; } = 30000;
		internal List<ChartData> HistoricValues { get; set; } = new List<ChartData>();
		internal string LogoSource { get; set; }
	}
}
