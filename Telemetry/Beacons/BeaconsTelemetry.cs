using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Telemetry.Beacons
{
    public class BeaconsTelemetry
    {
        public int Type { get; set; }
        public TelemetryData Data { get; set; }
    }

    public class TelemetryData
    {
        public string UserId { get; set; }
        public IList<APoint> APoints { get; set; }
    }

    public class APoint
    {
        public string Uid { get; set; }
        public IList<SingleBeaconTelemetry> Beacons { get; set; }
    }


    public class SingleBeaconTelemetry
    {
        public string Mac { get; set; }
        public IList<TelemetryValues> Values { get; set; }
    }

    public class TelemetryValues
    {
        public int Rssi { get; set; }
        public DateTime Time { get; set; }
    }
}
