using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;

namespace RangerTest.Infrastructure
{
    public class BaseRangerSettings : IRangerSettings
    {
        private readonly IEnumerable<IBeaconBody> _firstLine = new List<IBeaconBody>
        {
            BeaconBody.FromMac("64:cf:d9:2b:9f:8d"),
            BeaconBody.FromMac("c4:f3:12:62:c0:44")

        };

        private readonly IEnumerable<IBeaconBody> _secondLine = new List<IBeaconBody>
        {
            BeaconBody.FromMac("0C:B2:B7:3F:2B:53"),
            BeaconBody.FromMac("0c:b2:b7:3e:55:f8")
            
        };

        public int GetActualSignalCount()
        {
            return 1;
        }

        public int GetActualSignalPeriod()
        {
            return 1000;
        }

        public IEnumerable<IBeaconBody> GetBeaconsBySpace(string accessPoint, BeaconLine line)
        {
            switch(line)
            {
                case BeaconLine.First:
                    return _firstLine;

                case BeaconLine.Second:
                    return _secondLine;

                default: return new List<IBeaconBody>();
            }
        }
    }
}
