using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Enums;

namespace Trigger.Interfaces
{
    public interface IRangerSettings
    {
        IEnumerable<IBeaconBody> GetBeacons(string accessPoint, BeaconLine line);

        int GetSlideAverageCount(string accessPoint);
    }
}
