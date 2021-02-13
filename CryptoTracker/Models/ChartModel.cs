using Microsoft.Toolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using Telerik.Charting;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CryptoTracker.Models {
	public class ChartModel : ObservableObject {
        /// <summary>
        /// List of ChartData containing the values for the chart
        /// </summary>
        private List<ChartPoint> _chartData = new List<ChartPoint>() { new ChartPoint() { Value = 5, Date = DateTime.Today } };
        internal List<ChartPoint> ChartData {
            get => _chartData;
            set => SetProperty(ref _chartData, value);
        }

        /// <summary>
        /// Tuple with the minimum and maximum value to adjust the chart
        /// </summary>
        private (float Min, float Max) _pricesMinMax = (0, 100);
        internal (float Min, float Max) PricesMinMax {
            get => _pricesMinMax;
            set => SetProperty(ref _pricesMinMax, value);
        }

        /// <summary>
        /// Stroke and two semi-transparent fills to paint the charts
        /// </summary>
        private SolidColorBrush _chartStroke = (SolidColorBrush)App.Current.Resources["main_gray"];
        private Color _chartFill1 = Color.FromArgb(64, 128, 128, 128);
        private Color _chartFill2 = Color.FromArgb(16, 128, 128, 128);

        internal SolidColorBrush ChartStroke {
            get => _chartStroke;
            set {
                SetProperty(ref _chartStroke, value);
                var color = value.Color;
                ChartFill1 = Color.FromArgb(64, color.R, color.G, color.B);
                ChartFill2 = Color.FromArgb(16, color.R, color.G, color.B);
            }
        }
        internal Color ChartFill1 {
            get => _chartFill1;
            set => SetProperty(ref _chartFill1, value);
        }
        internal Color ChartFill2 {
            get => _chartFill2;
            set => SetProperty(ref _chartFill2, value);
        }

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
