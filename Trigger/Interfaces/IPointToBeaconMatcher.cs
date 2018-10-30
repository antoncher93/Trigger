using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes;
using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface IPointToBeaconMatcher
    {
        IEnumerable<string> GetBeacons(Telemetry telemetry);
    }
}
