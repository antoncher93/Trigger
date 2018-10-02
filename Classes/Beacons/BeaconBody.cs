using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Beacons
{
    public class BeaconBody : IBeaconBody
    {
        public string Mac { get; set; }

        private BeaconBody() { }

        public static BeaconBody FromMac(string mac)
        {
            return new BeaconBody { Mac = mac };
        }
    }
}