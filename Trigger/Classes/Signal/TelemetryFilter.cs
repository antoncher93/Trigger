using System.Collections.Generic;
using System.Linq;
using Trigger.Classes;
using Trigger.Interfaces;

namespace Trigger.Signal
{
    public static class TelemetryFilter
    {
        public static IEnumerable<Telemetry> CutNoise(this IEnumerable<Telemetry> telemetry, IPointToBeaconMatcher mather)
        {
            foreach (var t in telemetry)
            {
                foreach (var ap in t)
                {
                    IEnumerable<MacAddress> beacons = mather.GetBeacons(ap.Key).Select(x => (MacAddress)x);
                    ap.Value.Beacons = ap.Value.Beacons.Where(b => beacons.Contains(b.Address)).ToList();
                }

                yield return t;
            }
        }
    }
}
