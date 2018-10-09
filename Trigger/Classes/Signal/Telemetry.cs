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
    public class Telemetry : Dictionary<string, AccessPoint>
    {
        [JsonProperty(Order = 1)]  
        public string UserId { get; set; }

        public Telemetry Add(AccessPoint point)
        {
            Add(point.Uid, point);
            return this;
        }

        public Telemetry AddRange(params AccessPoint[] points)
        {
            foreach (var p in points)
                Add(p.Uid, p);

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

        public void NewBeacon(string mac, int rssi, string apointUid, DateTime time)
        {
            AccessPoint apoint = null;
            if (!this.ContainsKey(apointUid))
            {
                apoint = AccessPoint.FromUid(apointUid);
                this.Add(apoint);
            }
            apoint = this[apointUid];

            var beacon = apoint.Beacons.FirstOrDefault(b => b.Mac == mac);
            if (beacon == null)
            {
                beacon = Beacon.FromMac(mac);
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