using System;
using System.Collections.Generic;
using Trigger.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;

namespace Trigger.Test
{
    public class DummyRangerSettings : IRangerSettings
    {
        public IEnumerable<IBeaconBody> GetBeacons(string accessPoint, BeaconLine line)
        {
            switch (line)
            {
                case BeaconLine.Help:
                    return new[] { BeaconBody.FromMac("de:a6:78:08:52:a2") };
                case BeaconLine.First:
                    return new[] { BeaconBody.FromMac("c9:18:b1:cf:9b:50") };
                case BeaconLine.Second:
                    return new[] { BeaconBody.FromMac("e3:25:3e:0a:7e:4c") };
                default:
                    throw new ArgumentException("Unknown line type");
            }
        }

        public int GetSlideAverageCount(string accessPoint) => 5;
    }
}
