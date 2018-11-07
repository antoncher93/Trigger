using System;
using Trigger.Enums;

namespace Trigger.Signal
{
    public class TriggerEventArgs : EventArgs
    {
        public TriggerEventType Type { get; set; }
        public string SpaceUid { get; set; }
        public string UserId { get; set; }
        public DateTime Timespan { get; set; }

        public override string ToString()
        {
            return $"{Timespan.ToString("HH:mm:ss.fff")}: {Type}";
        }
    }
}