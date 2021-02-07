using System;
using System.Collections.Generic;

namespace CryptoTracker.Helpers {
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
    }
}
