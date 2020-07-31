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
    }
}
