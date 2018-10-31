using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes;
using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface ISpaceToBeaconMatcher
    {
        string GetSpaceUid(MacAddress beaconMacAddress);
    }
}
