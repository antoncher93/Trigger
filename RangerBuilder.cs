using System;
using Trigger.Telemetry;
using Trigger.Telemetry.Beacons;

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
            => Modify(() => { ranger.FirstLineBeacons.Add(beacon); });

        public RangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
            => Modify(() => { ranger.SecondLineBeacons.Add(beacon); });

        public RangerBuilder AddHelpBeacon(IBeaconBody beacon)
            => Modify(() => { ranger.HelpBeacons.Add(beacon); });

        public RangerBuilder SetCalcSlideAverageCount(int count)
            => Modify(() => { ranger.SetSlideAverageCount(count); });

        public RangerBuilder SetAPointUid(string uid)
            => Modify(() => { ranger.SetAPoint(new APoint { Uid = uid }); });

        public Ranger Build() => ranger;
    }
}