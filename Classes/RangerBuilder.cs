using System;
using Trigger.Beacons;
using Trigger.Classes;

namespace Trigger
{
    public class RangerBuilder
    {
        private Ranger ranger = new Ranger();

        private RangerBuilder Modify(Action act)
        {
            act?.Invoke();
            return this;
        }

        public RangerBuilder AddFirstLineBeacon(IBeaconBody beacon)
            => Modify(() => { ranger._firstLineBeacons.Add(beacon); });

        public RangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
            => Modify(() => { ranger._secondLineBeacons.Add(beacon); });

        public RangerBuilder AddHelpBeacon(IBeaconBody beacon)
            => Modify(() => { ranger._helpBeacons.Add(beacon); });

        public RangerBuilder SetCalcSlideAverageCount(int value)
            => Modify(() => { ranger.slideAverageCount = value; });

        public RangerBuilder SetAPointUid(string uid)
            => Modify(() => { ranger.apoint = new AccessPoint { Uid = uid }; });

        public RangerBuilder SetUser(string userUid)
            => Modify(() => { ranger._userUid = userUid.Trim(); });

        public Ranger Build()
        {
            if (string.IsNullOrWhiteSpace(ranger._userUid))
                throw new ArgumentException("User undeclared!");
            
            return ranger;
        }
    }
}