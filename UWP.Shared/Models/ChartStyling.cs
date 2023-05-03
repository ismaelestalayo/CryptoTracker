using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace UWP.Models {
    public partial class ChartStyling : ObservableObject {
        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
        /// </summary>
        [ObservableProperty]
        private string labelFormat = "{0:HH:mm}";

        [ObservableProperty]
        private double gapLength = 0.98;

    }
}
