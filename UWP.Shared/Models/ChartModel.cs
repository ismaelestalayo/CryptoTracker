using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace UWP.Models {
	public class ChartModel : ObservableObject {
        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        private List<ChartPoint> _chartData = new List<ChartPoint>() { new ChartPoint() { Value = 5, Date = DateTime.Today } };
        public List<ChartPoint> ChartData {
            get => _chartData;
            set => SetProperty(ref _chartData, value);
        }

        /// <summary>
        /// Tuple with the minimum and maximum value to adjust the chart
        /// </summary>
        private (float Min, float Max) _pricesMinMax = (0, 100);
        public (float Min, float Max) PricesMinMax {
            get => _pricesMinMax;
            set => SetProperty(ref _pricesMinMax, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush _chartStroke = new SolidColorBrush(Color.FromArgb(127, 127, 127, 127));
        private Color _chartFill1 = Color.FromArgb(64, 128, 128, 128);
        private Color _chartFill2 = Color.FromArgb(16, 128, 128, 128);

        public SolidColorBrush ChartStroke {
            get => _chartStroke;
            set {
                SetProperty(ref _chartStroke, value);
                var color = value.Color;
                ChartFill1 = Color.FromArgb(64, color.R, color.G, color.B);
                ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
            }
        }
        public Color ChartFill1 {
            get => _chartFill1;
            set => SetProperty(ref _chartFill1, value);
        }
        public Color ChartFill2 {
            get => _chartFill2;
            set => SetProperty(ref _chartFill2, value);
        }

        /// <summary>
        /// Attributes to adjust the axis to the plotted time interval
        /// </summary>
        private string _labelFormat = "{0:HH:mm}";
        public string LabelFormat {
            get => _labelFormat;
            set => SetProperty(ref _labelFormat, value);
        }

        private TimeInterval _majorStepUnit = TimeInterval.Minute;
        public TimeInterval MajorStepUnit {
            get => _majorStepUnit;
            set => SetProperty(ref _majorStepUnit, value);
        }

        private int _majorStep = 10;
        public int MajorStep {
            get => _majorStep;
            set => SetProperty(ref _majorStep, value);
        }

        private DateTime _minimum = DateTime.Now.AddHours(-1);
        public DateTime Minimum {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
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
