using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Telerik.Charting;

namespace UWP.Models {
    public partial class ChartStyling : ObservableObject {
        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
        /// </summary>
        [ObservableProperty]
        private string labelFormat = "{0:HH:mm}";

        [ObservableProperty]
        private TimeInterval majorStepUnit = TimeInterval.Minute;

        [ObservableProperty]
        private int majorStep = 10;

        [ObservableProperty]
        private DateTime minimum = DateTime.Now.AddHours(-1);

        [ObservableProperty]
        private int tickInterval = 10;
    }
}
