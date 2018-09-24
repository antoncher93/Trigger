using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Telemetry.Beacons
{
    public interface IBeaconBody
    {
        //string Uid { get; set; }
        //string Serial { get; set; }
        //string LineType { get; set; }
        string Mac { get; set; }
    }
}
