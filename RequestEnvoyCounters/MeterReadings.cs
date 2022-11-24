using System;
using System.Collections.Generic;
using System.Text;

namespace RequestEnvoyCounters
{
    // List<MeterReading> meterReadings = JsonConvert.DeserializeObject<List<MeterReading>>(myJsonResponse);
    public class Channel
    {
        public int eid { get; set; }
        public int timestamp { get; set; }
        public double actEnergyDlvd { get; set; }
        public double actEnergyRcvd { get; set; }
        public double apparentEnergy { get; set; }
        public double reactEnergyLagg { get; set; }
        public double reactEnergyLead { get; set; }
        public double instantaneousDemand { get; set; }
        public double activePower { get; set; }
        public double apparentPower { get; set; }
        public double reactivePower { get; set; }
        public double pwrFactor { get; set; }
        public double voltage { get; set; }
        public double current { get; set; }
        public double freq { get; set; }
    }

    public class MeterReading
    {
        public int eid { get; set; }
        public int timestamp { get; set; }
        public double actEnergyDlvd { get; set; }
        public double actEnergyRcvd { get; set; }
        public double apparentEnergy { get; set; }
        public double reactEnergyLagg { get; set; }
        public double reactEnergyLead { get; set; }
        public double instantaneousDemand { get; set; }
        public double activePower { get; set; }
        public double apparentPower { get; set; }
        public double reactivePower { get; set; }
        public double pwrFactor { get; set; }
        public double voltage { get; set; }
        public double current { get; set; }
        public double freq { get; set; }
        public List<Channel> channels { get; set; }
    }


}
