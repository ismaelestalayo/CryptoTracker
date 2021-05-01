using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
    public class ChartModel : ObservableObject {
        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        private List<ChartPoint> chartData = new List<ChartPoint>();
        public List<ChartPoint> ChartData {
            get => chartData;
            set => SetProperty(ref chartData, value);
        }

        /// <summary>
        /// Tuple with the minimum and maximum value to adjust the chart
        /// </summary>
        private (double Min, double Max) pricesMinMax = (0, 100);
        public (double Min, double Max) PricesMinMax {
            get => pricesMinMax;
            set => SetProperty(ref pricesMinMax, value);
        }

        private double volumeMax = 0;
        public double VolumeMax {
            get => volumeMax;
            set => SetProperty(ref volumeMax, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush chartStroke = new SolidColorBrush(Color.FromArgb(127, 127, 127, 127));

        public SolidColorBrush ChartStroke {
            get => chartStroke;
            set => SetProperty(ref chartStroke, value);
        }

        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
        /// </summary>
        private string labelFormat = "{0:HH:mm}";
        public string LabelFormat {
            get => labelFormat;
            set => SetProperty(ref labelFormat, value);
        }

        private TimeInterval majorStepUnit = TimeInterval.Minute;
        public TimeInterval MajorStepUnit {
            get => majorStepUnit;
            set => SetProperty(ref majorStepUnit, value);
        }

        private int majorStep = 10;
        public int MajorStep {
            get => majorStep;
            set => SetProperty(ref majorStep, value);
        }

        private DateTime minimum = DateTime.Now.AddHours(-1);
        public DateTime Minimum {
            get => minimum;
            set => SetProperty(ref minimum, value);
        }

        private int tickInterval = 10;
        public int TickInterval {
            get => tickInterval;
            set => SetProperty(ref tickInterval, value);
        }

        /// <summary>
        /// 
        /// </summary>
        private string timeSpan = "1w";
        public string TimeSpan {
            get => timeSpan;
            set => SetProperty(ref timeSpan, value);
        }
    }
}
