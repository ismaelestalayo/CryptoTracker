using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace CryptoTracker.UserControls {
	public sealed partial class TimeRangeRadioButtons : UserControl {
		public TimeRangeRadioButtons() {
			this.InitializeComponent();
		}

		private void UpdateTimeSpan(object sender, RoutedEventArgs e) {
			timeSpan = ((ContentControl)sender).Content.ToString();
		}

		private string timeSpan = "1w";
		public string TimeSpan {
			get => timeSpan;
		}
	}
}
