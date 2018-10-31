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

        ///// <summary>
        ///// Cut noise beacons from package
        ///// </summary>
        ///// <param name="telemetry"></param>
        ///// <param name="matcher"></param>
        ///// <returns></returns>
        //public static IEnumerable<Telemetry> CutNoise(this IEnumerable<Telemetry> telemetry, IPointToBeaconMatcher matcher)
        //{
        //    foreach(var t in telemetry)
        //    {
        //        IEnumerable<MacAddress> beacons = matcher.GetBeacons(t).Select(x => (MacAddress)x);
        //        //ap.Beacons = ap.Beacons.Where(b => beacons.Contains(b.Address)).ToList();

        //        yield return t;
        //    }
        //}


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
