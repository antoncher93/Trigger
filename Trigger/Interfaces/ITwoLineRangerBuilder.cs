using System;
using Trigger.Beacons;

namespace Trigger.Interfaces
{
    public interface ITwoLineRangerBuilder : IRangerBuilder<ITwoLineRanger> 
    {
        ITwoLineRangerBuilder AddFirstLineBeacon(IBeaconBody beacon);
        ITwoLineRangerBuilder AddHelpBeacon(IBeaconBody beacon);
        ITwoLineRangerBuilder AddSecondLineBeacon(IBeaconBody beacon);
        ITwoLineRangerBuilder Modify(Action act);
        ITwoLineRangerBuilder SetActualSignalCount(int count);
        ITwoLineRangerBuilder SetActualSignalPeriod(int milliseconds);
        ITwoLineRangerBuilder SetSpaceUid(string uid);
    }
}