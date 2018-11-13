using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.Classes;
using Trigger.Interfaces;

namespace Trigger.Signal
{
    public static class TelemetryFilters
    {
        private const int MIN_STABLE_TIME_SEC = 10;

        /// <summary>
        /// Cut noise beacons from package
        /// </summary>
        /// <param name="telemetry"></param>
        /// <returns></returns>
        public static IEnumerable<Telemetry> CutNoise(this IEnumerable<Telemetry> telemetry)
        {
            Guid g = new Guid();
            foreach (var t in telemetry)
            {
                var notGuid = t.Where(x => !Guid.TryParse(x.Address, out g)).ToList();
                foreach (var e in notGuid)
                    t.Remove(e);

                yield return t;
            }
        }


        /// <summary>
        /// Get only telemetry with stable last event
        /// </summary>
        /// <returns>Only time-stable telemetry</returns>
        public static IEnumerable<Telemetry> GetFinalized(this IEnumerable<Telemetry> telemetry)
        {
            foreach (var t in telemetry)
            {
                if (t.LastTimestamp.HasValue 
                    && (DateTime.Now - t.LastTimestamp.Value).TotalSeconds >= MIN_STABLE_TIME_SEC)
                    yield return t;
            }
        }
    }
}
