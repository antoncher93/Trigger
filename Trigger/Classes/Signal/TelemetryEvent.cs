using System;
using Trigger.Classes;

namespace Trigger.Signal
{
    public class TriggerEventArgs : EventArgs
    {
        public TriggerEventArgs(AccessPoint apoint, DateTime time, string userId)
        {
            APoint = apoint;
            DateTime = time;
            UserId = userId;
        }

        public AccessPoint APoint { get; private set; }
        public DateTime DateTime { get; private set; }
        public string UserId { get; private set; }
    }
}
