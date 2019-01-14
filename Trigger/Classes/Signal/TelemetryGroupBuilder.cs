using System;
using System.Collections.Generic;
using Trigger.Signal;
using System.Linq;
using Trigger.Interfaces;

namespace Trigger.Classes.Signal
{
    public class TelemetryGroupBuilder
    {
        private IList<Telemetry> _items = new List<Telemetry>();
        private IList<IRanger> _observers = new List<IRanger>();

        public TelemetryGroupBuilder Add(Telemetry telemetry)
        {
            // HardCoded
            if (telemetry.UserId == null)
                telemetry.UserId = "19377fca-2d5f-4d8a-85f6-8adb0a1e8e12";

            Telemetry existed = _items.FirstOrDefault(i => string.Equals(i.UserId, telemetry.UserId, StringComparison.InvariantCultureIgnoreCase));

            if (existed != null)
                existed.Append(telemetry);
            else _items.Add(telemetry);

            return this;
        }

        public TelemetryGroupBuilder AddRange(IEnumerable<Telemetry> telemetries)
        {
           foreach (var s in telemetries)
                Add(s);

            return this;
        }

        public TelemetryGroupBuilder AddObserverRangers(ISpaceToBeaconMatcher matcher, Func<string, IRanger> getObserver)
        {
            _observers.Clear();

            _observers =
                _items.SelectMany(t => t.Select(b => matcher.GetSpaceUid(b.Address))).Distinct() // If beacon address is macAddress than b.Address only
                    .Select(spaceUid => getObserver(spaceUid))
                    .ToList();

            _observers.Remove(null);

            return this;
        }

        public TelemetryGroup Build()
        {
            TelemetryGroup result = new TelemetryGroup(_items);

            foreach (var ranger in _observers)
                ranger.Subscribe(result);

            return result;
        }
    }
}