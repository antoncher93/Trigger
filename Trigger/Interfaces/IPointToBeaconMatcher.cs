using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes;

namespace Trigger.Interfaces
{
    public interface IPointToBeaconMatcher
    {
        IEnumerable<string> GetBeacons(string accessPointUid);
    }
}
