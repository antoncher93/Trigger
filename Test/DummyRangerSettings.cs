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
                    return new[] { BeaconBody.FromMac("00de:a6:78:08:52:a2") };
                case BeaconLine.First:
                    return new[] { BeaconBody.FromMac("DF:20:C6:5A:62:5F")};
                case BeaconLine.Second:
                    return new[] { BeaconBody.FromMac("DE:A6:78:08:52:A2") };
                default:
                    throw new ArgumentException("Unknown line type");
            }
        }

        public int GetSlideAverageCount(string accessPoint) => 5;
    }
}
