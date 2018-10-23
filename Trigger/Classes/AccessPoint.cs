using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Classes.Beacons;
using Trigger.Signal;

namespace Trigger.Classes
{
    public class AccessPointData
    {
        [JsonIgnore]
        public string AccessPointUid { get; set; }
        public IList<BeaconData> Beacons { get; set; } = new List<BeaconData>();
        public static AccessPointData FromUid(string uid)
        {
            return new AccessPointData
            {
                AccessPointUid = uid,
                Beacons = new List<BeaconData>()
            };
        }
        public void Append(AccessPointData apoint)
        {
            if (string.Equals(AccessPointUid, apoint.AccessPointUid, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var beacon in apoint.Beacons)
                {
                    BeaconData res = Beacons.FirstOrDefault(b => string.Equals(b.Address, beacon.Address, StringComparison.CurrentCultureIgnoreCase));
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
