using Trigger.Telemetry;
using Trigger.Telemetry.Beacons;

namespace Trigger
{
    public class RangerBuilder
    {
        private Ranger ranger = new Ranger();

        public RangerBuilder AddFirstLineBeacon(IBeaconBody beacon)
        {
            ranger.FirstLineBeacons.Add(beacon);
            return this;
        }

        public RangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
        {
            ranger.SecondLineBeacons.Add(beacon);
            return this;
        }

        public RangerBuilder AddHelpBeacon(IBeaconBody beacon)
        {
            ranger.HelpBeacons.Add(beacon);
            return this;
        }

        public RangerBuilder SetCalcSlideAverageCount(int count)
        {
            ranger.SetSlideAverageCount(count);
            return this;
        }

        public RangerBuilder SetAPointUid(string uid)
        {
            ranger.SetAPoint(new APoint { Uid = uid });
            return this;
        }

        public Ranger Build() => ranger;
    }
}