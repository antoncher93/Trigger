using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Signal;

namespace Trigger.Classes
{
    public class AccessPoint
    {
        [JsonIgnore]
        public string Uid { get; set; }
        public IList<SingleBeaconTelemetry> Beacons { get; set; }
        public static AccessPoint FromUid(string uid)
        {
            return new AccessPoint
            {
                Uid = uid,
                Beacons = new List<SingleBeaconTelemetry>()
            };
        }
        public void Append(AccessPoint apoint)
        {
            if (string.Equals(Uid, apoint.Uid, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var beacon in apoint.Beacons)
                {
                    SingleBeaconTelemetry res = Beacons.FirstOrDefault(b => string.Equals(b.Mac, beacon.Mac, StringComparison.CurrentCultureIgnoreCase));
                    if (res == null)
                    {
                        Beacons.Add(beacon);
                    }
                    else
                    {
                        res.Append(beacon);
                    }
                }
            }
        }
    }
}
