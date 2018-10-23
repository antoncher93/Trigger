using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Trigger.Signal
{
    public class TelemetryGroup : ConcurrentDictionary<string, Telemetry>
    {
        public void Add(Telemetry item)
        {
            // HardCoded
            if (item.UserId == null)
                item.UserId = "19377fca-2d5f-4d8a-85f6-8adb0a1e8e12";
                
            AddOrUpdate(item.UserId, item, (userId, telemetry) =>
            {
                Telemetry existed = this[item.UserId];
                existed.Append(telemetry);
                return existed;
            });
        }

        public void Bind(IEnumerable<Telemetry> signals)
        {
            foreach (var s in signals)
                Add(s);
        }
    }
}