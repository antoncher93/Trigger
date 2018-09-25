using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Telemetry.Beacons;

namespace Trigger.Interfaces
{
    public interface IRangerEvents
    {
        event EventHandler<TriggerEventArgs> Enter;
        event EventHandler<TriggerEventArgs> Exit;
        event EventHandler<TriggerEventArgs> EnterByPeaks;
    }
}
