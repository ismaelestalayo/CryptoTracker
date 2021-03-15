using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using Telerik.Charting;

namespace UWP.Models {
	public class ChartStyling : ObservableObject {
        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
        /// </summary>
        private string _labelFormat = "{0:HH:mm}";
        internal string LabelFormat {
            get => _labelFormat;
            set => SetProperty(ref _labelFormat, value);
        }

        private TimeInterval _majorStepUnit = TimeInterval.Minute;
        internal TimeInterval MajorStepUnit {
            get => _majorStepUnit;
            set => SetProperty(ref _majorStepUnit, value);
        }

        private int _majorStep = 10;
        internal int MajorStep {
            get => _majorStep;
            set => SetProperty(ref _majorStep, value);
        }

        private DateTime _minimum = DateTime.Now.AddHours(-1);
        internal DateTime Minimum {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }
	}
}
