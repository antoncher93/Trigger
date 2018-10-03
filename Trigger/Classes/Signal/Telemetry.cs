using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Trigger.Classes;
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
            if (this[apointUid] == null)
                this[apointUid] = AccessPoint.FromUid(apointUid);

            AccessPoint apoint = this[apointUid];
            //var apoint = Data.FirstOrDefault(ap => ap.Value.Uid == apointUid).Value;
            //if(apoint == null)
            //{
            //    apoint = AccessPoint.FromUid(apointUid);
            //    Data.Add(apoint);
            //}

            var beacon = apoint.Beacons.FirstOrDefault(b => b.Mac == mac);
            if (beacon == null)
            {
                beacon = SingleBeaconTelemetry.FromMac(mac);
                apoint.Beacons.Add(beacon);
            }
            var rssivalue = beacon.Values.FirstOrDefault(v => v.Time == time);
            if (rssivalue == null)
            {
                rssivalue = new RssiValue { Rssi = rssi, Time = time };
                beacon.Values.Add(rssivalue);
                _lastSignalTime = time;
            }
        }

        public static implicit operator Telemetry(string s)
        {
            return JsonConvert.DeserializeObject<Telemetry>(s);
        }
    }

    public class SingleBeaconTelemetry
    {
        public string Mac { get; set; }
        public IList<RssiValue> Values { get; set; }

        public static SingleBeaconTelemetry FromMac(string mac)
        {
            return new SingleBeaconTelemetry
            {
                Mac = mac,
                Values = new List<RssiValue>()
            };
        }

        public void CleanBefore(DateTime time)
        {
            for (int i = 0; i < Values.Count; i++)
            {
                if (Values[i].Time < time)
                {
                    Values.RemoveAt(i);
                    i--;

                }
            }
        }

        public void Append(SingleBeaconTelemetry beacon)
        {
            if (string.Equals(this.Mac, beacon.Mac, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach (var rssi in beacon.Values)
                {
                    if (!this.Values.Any(b => b.Time.Equals(rssi.Time)))
                    {
                        this.Values.Add(rssi);
                    }
                }
            }
        }
    }

    public class RssiValue
    {
        public int Rssi { get; set; }
        public DateTime Time { get; set; }
    }
}