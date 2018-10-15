using System;
using System.Collections.Generic;
using Trigger.Beacons;
using Trigger.Classes.Logging;
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
                    return new List<BeaconBody>();
                case BeaconLine.First:
                    return new[] { BeaconBody.FromMac("DF:20:C6:5A:62:5F"), BeaconBody.FromMac("c9:18:b1:cf:9b:50") };
                case BeaconLine.Second:
                    return new[] { BeaconBody.FromMac("E3:25:3E:0A:7E:4C"), BeaconBody.FromMac("de:a6:78:08:52:a2") };
                default:
                    throw new ArgumentException("Unknown line type");
            }
        }

        public int GetActualPeriod()
        {
            return 1000;
        }

        public ILogger GetLogger()
        {
            return null;
        }
    }

    public class Dummy2RangerSettings : IRangerSettings
    {
        public IEnumerable<IBeaconBody> GetBeacons(string accessPoint, BeaconLine line)
        {
            switch (line)
            {
                case BeaconLine.Help:
                    return new List<BeaconBody>();
                case BeaconLine.First:
                    return new[] { BeaconBody.FromMac("de:a6:78:08:52:a2"), BeaconBody.FromMac("C4:7B:88:EE:69:EE") };
                case BeaconLine.Second:
                    return new[] { BeaconBody.FromMac("DF:20:C6:5A:62:5F"), BeaconBody.FromMac("c9:18:b1:cf:9b:50") };
                default:
                    throw new ArgumentException("Unknown line type");
            }
        }

        public int GetActualPeriod()
        {
            return 1000;
        }

        public ILogger GetLogger()
        {
            return new DBLogger();
        }
    }
}
