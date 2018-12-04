using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Interfaces;

namespace Trigger.Classes
{
    public class PeackRangerBuilder : IRangerBuilder
    {
        private PeakRanger ranger;

        public PeackRangerBuilder()
        {
            ranger = new PeakRanger();
        }

        private PeackRangerBuilder Modify(Action action)
        {
            action.Invoke();
            return this;
        }

        public PeackRangerBuilder AddFirstLineBeacon(IBeaconBody beacon)
            => Modify(() => ranger._firstLineBeacons.Add(beacon));

        public PeackRangerBuilder AddSecondLineBeacon(IBeaconBody beacon)
            => Modify(() => ranger._secondLineBeacons.Add(beacon));

        public IRanger Build() => ranger;
    }
}
