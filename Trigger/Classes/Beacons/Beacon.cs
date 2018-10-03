using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Beacons
{
    public class Beacon
    {
        public string Mac { get; set; }
        public int Rssi { get; set; }
        public DateTime DateTime { get; set; }
    }
}
