using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Interfaces;

namespace Trigger.Rangers
{
    public class TwoLineRangerBuilder : BaseRangerBuilder
    {
        public new TwoLineRangerBuilder Modify(Action act)
        {
            return (TwoLineRangerBuilder)base.Modify(act);
        }

        public TwoLineRangerBuilder AddFirstLineBeacon(IBeaconBody beacon)
           => Modify(() => { (ranger as TwoLineRanger)._firstLineBeacons.Add(beacon); });

        public TwoLineRangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
            => Modify(() => { (ranger as TwoLineRanger)._secondLineBeacons.Add(beacon); });

        public TwoLineRangerBuilder AddHelpBeacon(IBeaconBody beacon)
            => Modify(() => { (ranger as TwoLineRanger)._helpBeacons.Add(beacon); });

        public BaseRangerBuilder SetSpaceUid(string uid)
            => Modify(() => { (ranger as TwoLineRanger)._spaceUid = uid; });
    }
}
