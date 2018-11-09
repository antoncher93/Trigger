using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Classes.Logging;
using Trigger.Enums;

namespace Trigger.Interfaces
{
    public interface IRangerSettings
    {
        IEnumerable<IBeaconBody> GetBeaconsBySpace(string accessPoint, BeaconLine line);
   //     int GetActualPeriod();
      //  ILogger GetLogger();
    }
}
