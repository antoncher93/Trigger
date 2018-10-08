using System;
using Trigger.Enums;

namespace Trigger.Signal
{
    public class TriggerEventArgs : EventArgs
    {
        public TriggerEventType Type { get; set; }
        public string AccessPointUid { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
