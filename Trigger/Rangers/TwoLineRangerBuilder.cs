using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Interfaces;

namespace Trigger.Rangers
{
    public class TwoLineRangerBuilder : BaseRangerBuilder<ITwoLineRanger>, ITwoLineRangerBuilder
    {
        public new ITwoLineRangerBuilder Modify(Action act)
        {            
            return (TwoLineRangerBuilder)base.Modify(act);
        }

        private TwoLineRanger _tlRanger => (TwoLineRanger)ranger;

        public ITwoLineRangerBuilder AddFirstLineBeacon(IBeaconBody beacon)
           => Modify(() => { _tlRanger._firstLineBeacons.Add(beacon); });

        public ITwoLineRangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
            => Modify(() => { _tlRanger._secondLineBeacons.Add(beacon); });

        public ITwoLineRangerBuilder AddHelpBeacon(IBeaconBody beacon)
            => Modify(() => { _tlRanger._helpBeacons.Add(beacon); });

        public ITwoLineRangerBuilder SetActualSignalCount(int count)
            => Modify(() => _tlRanger._actualSignalCount = count);

        public ITwoLineRangerBuilder SetActualSignalPeriod(int milliseconds)
            => Modify(() => _tlRanger._actualSignalPeriod = milliseconds);

        public ITwoLineRangerBuilder SetSpaceUid(string uid)
            => Modify(() => _tlRanger._spaceUid = uid);
    }
}
