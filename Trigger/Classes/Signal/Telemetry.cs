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
    public class Telemetry : Dictionary<string, Beacon>
    {
        [JsonProperty(Order = 1)]  
        public string UserId { get; set; }

        public Telemetry Add(Beacon beacon)
        {
            Add(beacon.Mac, beacon);
            return this;
        }

        public Telemetry AddRange(params Beacon[] beacons)
        {
            foreach (var b in beacons)
                Add(b.Mac, b);

            return this;
        }

        public TelemetryType Type { get; set; } = TelemetryType.FromUser;

        public DateTime MinDateTime
        {
            get
            {
                return Values.SelectMany(b => b.Select(bi => bi.Time)).Min();
            }
        }


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

        public void NewBeacon(string mac, int rssi, DateTime time)
        {
            Beacon beacon = null;
            if (!this.ContainsKey(mac))
            {
                beacon = Beacon.FromMac(mac);
                this.Add(beacon);
            }
            beacon = this[mac];
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