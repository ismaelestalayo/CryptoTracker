using UWP.Models;
using System;
using System.Collections.Generic;

namespace UWP.Helpers {
    class GraphHelper {
        internal static (float Min, float Max) GetMinMaxOfArray(List<float> historic) {
            if (historic.Count == 0)
                return (0, 10);
			
            float min = historic[0];
            float max = historic[0];

            foreach (float h in historic) {
                if (h < min)
                    min = h;
                else if (h > max)
                    max = h;
            }

            double diff = max - min;

            min -= (float)(diff * 0.2);
            max += (float)(diff * 0.1);

            if (min < 0)
                min = 0;

            return (min, max);
        }

        /// <summary>
        /// Parse TimeSpan from RadioButtons to values for GetHistoric
        /// </summary>
        internal static readonly Dictionary<string, (string, int, int)> TimeSpanParser =
            new Dictionary<string, (string, int, int)>() {
                { "1h",  ("minute",   60, 1) }, // 1 h ----------->   60 mins
                { "4h",  ("minute",  240, 1) }, // 4 h = 60 * 4 -->  240 mins
                { "1d",  ("minute", 1440, 5) }, // 1 d = 60 * 24 -> 1440 mins (300 * 5)
                { "3d",  ("hour",     72, 1) }, // 3 d = 24 * 3 -->   72 hours 
                { "1w",  ("hour",    168, 1) }, // 1 w = 24 * 7 -->  168 hours
                { "1m",  ("hour",    372, 2) }, // 1 m = 24 * 31 ->  744 hours (372 * 2)
                { "3m",  ("day",      93, 1) }, // 3 m = 31 * 3 -->   93 days
                { "1y",  ("day",     365, 1) }, // 1 y ----------->  365 days
                { "all", ("day",       0, 1) }
            };

        /// <summary>
        /// Adjust a chart's axis 
        /// </summary>
        internal static ChartStyling AdjustLinearAxis(ChartStyling chartStyle, string timeSpan) {
            switch (timeSpan) {
                case "1h":
                    chartStyle.LabelFormat = "{0:HH:mm}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    chartStyle.MajorStep = 10;
                    chartStyle.Minimum = DateTime.Now.AddHours(-1);
                    break;

                case "4h":
                    chartStyle.LabelFormat = "{0:HH:mm}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Minute;
                    chartStyle.MajorStep = 30;
                    chartStyle.Minimum = DateTime.Now.AddHours(-4);
                    break;

                case "1d":
                    chartStyle.LabelFormat = "{0:HH:mm}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    chartStyle.MajorStep = 3;
                    chartStyle.Minimum = DateTime.Now.AddDays(-1);
                    break;

                case "3d":
                    chartStyle.LabelFormat = "{0:HH:mm}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Hour;
                    chartStyle.MajorStep = 6;
                    chartStyle.Minimum = DateTime.Now.AddDays(-3);
                    break;

                default:
                case "1w":
                    chartStyle.LabelFormat = "{0:ddd d}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Day;
                    chartStyle.MajorStep = 1;
                    chartStyle.Minimum = DateTime.Now.AddDays(-7);
                    break;

                case "1m":
                    chartStyle.LabelFormat = "{0:d/M}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    chartStyle.MajorStep = 1;
                    chartStyle.Minimum = DateTime.Now.AddMonths(-1);
                    break;

                case "3m":
                    chartStyle.LabelFormat = "{0:d/M}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Week;
                    chartStyle.MajorStep = 1;
                    chartStyle.Minimum = DateTime.Now.AddMonths(-3);
                    break;

                case "1y":
                    chartStyle.LabelFormat = "{0:MMM}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    chartStyle.MajorStep = 1;
                    chartStyle.Minimum = DateTime.MinValue;
                    break;

                case "all":
                    chartStyle.LabelFormat = "{0:MMM}";
                    chartStyle.MajorStepUnit = Telerik.Charting.TimeInterval.Month;
                    chartStyle.MajorStep = 6;
                    chartStyle.Minimum = DateTime.MinValue;
                    break;
            }
            return chartStyle;
        }
    }
}
