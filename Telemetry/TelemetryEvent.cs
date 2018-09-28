using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Telemetry
{
    public class TriggerEventArgs : EventArgs
    {
        public TriggerEventArgs(APoint apoint, DateTime time, string userId)
        {
            APoint = apoint;
            DateTime = time;
            UserId = userId;
        }
        public APoint APoint { get; private set; }
        public DateTime DateTime { get; private set; }
        public string UserId { get; private set; }
    }
}
