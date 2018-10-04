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