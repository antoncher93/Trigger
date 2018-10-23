using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Enums;

namespace Trigger.Signal
{
    public sealed class Telemetry : Dictionary<string, AccessPointData>
    {
        [JsonProperty(Order = 1)]  
        public string UserId { get; set; }

        public Telemetry Add(AccessPointData point)
        {
            Add(point.AccessPointUid, point);
            return this;
        }

        public Telemetry AddRange(params AccessPointData[] points)
        {
            foreach (var p in points)
                Add(p.AccessPointUid, p);

            return this;
        }

        public TelemetryType Type { get; set; } = TelemetryType.FromUser;

        public DateTime MinDateTime
        {
            get
            {
                return Values.SelectMany(a => a.Beacons.SelectMany(b => b.Select(bi => bi.Time))).Min();
            }
        }

        private DateTime _lastSignalTime;

        public void Append(Telemetry telemetry)
        {
            if (!string.Equals(telemetry.UserId, UserId, StringComparison.CurrentCulture))
                return;

            foreach (var apoint in telemetry)
            {
                this[apoint.Key].Append(apoint.Value);
                //AccessPoint res = Data.FirstOrDefault(ap => string.Equals(ap.Uid, apoint.Uid));
                //if(res == null)
                //{
                //    Data.APoints.Add(apoint);
                //}
                //else
                //{
                //    res.Append(apoint);
                //}
            }
        }

        public static Telemetry EmptyForUser(string userId)
        {
            return new Telemetry
            {
                Type = TelemetryType.FromUser,
                UserId = userId
            };
        }

        public void NewBeacon(MacAddress mac, int rssi, string apointUid, DateTime time)
        {
            AccessPointData apoint = null;
            if (!this.ContainsKey(apointUid))
            {
                apoint = AccessPointData.FromUid(apointUid);
                this.Add(apoint);
            }
            apoint = this[apointUid];

            var beacon = apoint.Beacons.FirstOrDefault(b => b.Address == mac);
            if (beacon == null)
            {
                beacon = BeaconData.FromMac(mac);
                apoint.Beacons.Add(beacon);
            }

            beacon.Add(new BeaconItem { Rssi = rssi, Time = time });
          //  var rssivalue = beacon.FirstOrDefault(v => v.Time == time);
         //   if (rssivalue == null)
         //   {
          //      rssivalue = new BeaconItem { Rssi = rssi, Time = time };
          //      beacon.Add(rssivalue);
          //      _lastSignalTime = time;
          //  }
        }

        public static implicit operator Telemetry(string s)
        {
            return JsonConvert.DeserializeObject<Telemetry>(s);
        }
    }
}