using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.Classes;
using Trigger.Enums;

namespace Trigger.Signal
{
    public class Telemetry
    {
        public TelemetryType Type { get; set; } = TelemetryType.FromUser;
        public TelemetryData Data { get; set; }

        public void Append(Telemetry telemetry)
        {
            if(string.Equals(telemetry.Data.UserId, this.Data.UserId, StringComparison.CurrentCulture))
            {
                foreach(var apoint in telemetry.Data.APoints)
                {
                    AccessPoint res = Data.APoints.FirstOrDefault(ap => string.Equals(ap.Uid, apoint.Uid));
                    if(res == null)
                    {
                        Data.APoints.Add(apoint);
                    }
                    else
                    {
                        res.Append(apoint);
                    }
                } 
            }
        }

        public static Telemetry EmptyForUser(string userId)
        {
            return new Telemetry
            {
                Type = TelemetryType.FromUser,
                Data = new TelemetryData
                {
                    UserId = userId
                }
            };
        }

        public void NewBeacon(string mac, int rssi, string apointUid, DateTime time)
        {
            var apoint = Data.APoints.FirstOrDefault(ap => ap.Uid == apointUid);
            if(apoint == null)
            {
                apoint = AccessPoint.FromUid(apointUid);
                Data.APoints.Add(apoint);
            }

            var beacon = apoint.Beacons.FirstOrDefault(b => b.Mac == mac);
            if(beacon == null)
            {
                beacon = SingleBeaconTelemetry.FromMac(mac);
                apoint.Beacons.Add(beacon);
            }
            var rssivalue = beacon.Values.FirstOrDefault(v => v.Time == time);
            if(rssivalue == null)
            {
                rssivalue = new RssiValue { Rssi = rssi, Time = time };
            }
        }

        public static implicit operator Telemetry(string s)
        {
            return JsonConvert.DeserializeObject<Telemetry>(s);
        }
    }

    public class TelemetryData
    {
        public string UserId { get; set; }
        public IList<AccessPoint> APoints { get; set; } = new List<AccessPoint>();
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

        public void Append(SingleBeaconTelemetry beacon)
        {
            if(string.Equals(this.Mac, beacon.Mac, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach(var rssi in beacon.Values)
                {
                    if(!this.Values.Any(b => b.Time.Equals(rssi.Time)))
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