using System;
using System.Collections.Generic;

namespace CryptoTracker.Helpers {
    class GraphHelper {

        internal static float GetMaximum(List<JSONhistoric> historic) {
            float max = historic[0].High;

            foreach (JSONhistoric h in historic)
                if (h.High > max)
                    max = h.High;

            return max;
        }

        internal static float GetMinimum(List<JSONhistoric> historic) {
            float min = historic[0].Low;

            foreach (JSONhistoric h in historic)
                if (h.Low < min)
                    min = h.Low;

            return min;
        }

        internal static float GetMaximumOfArray(List<float> historic) {
            float max = historic[0];

            foreach (float h in historic)
                if (h > max)
                    max = h;

            return max;
        }

        internal static float GetMinimumOfArray(List<float> historic) {
            float min = historic[0];

            foreach (float h in historic)
                if (h < min)
                    min = h;

            return min;
        }

        internal static (float min, float max) GetMinMaxOfArray(List<float> historic) {
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

            return (min, max);
        }
    }
}
