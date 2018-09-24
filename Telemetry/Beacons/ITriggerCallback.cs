using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Telemetry.Beacons
{
    public interface ITriggerCallback
    {
        void OnEnter(APoint apoint, DateTime time);
        void OnExit(APoint apoint, DateTime time);
    }
}
