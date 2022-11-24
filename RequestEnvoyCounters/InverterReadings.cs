using System;
using System.Collections.Generic;
using System.Text;

namespace RequestEnvoyCounters
{
    public class InverterReadings
    {
        public string serialNumber { get; set; }
        public int lastReportDate { get; set; }
        public int devType { get; set; }
        public int lastReportWatts { get; set; }
        public int maxReportWatts { get; set; }
    }
}
