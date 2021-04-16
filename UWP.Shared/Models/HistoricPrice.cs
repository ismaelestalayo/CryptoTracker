using System;

namespace UWP.Shared.Models {
    public class HistoricPrice {
        public int time { get; set; }
        public double high { get; set; } = 0;
        public double low { get; set; } = 0;
        public double open { get; set; } = 0;
        public double close { get; set; } = 0;
        public double volumefrom { get; set; } = 0;
        public double volumeto { get; set; } = 0;

        public double Average { get; set; } = 0;
        public string Date { get; set; }
        public DateTime DateTime { get; set; }
    }
}
