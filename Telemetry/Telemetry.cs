using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trigger.Telemetry
{
    public class TelemetryGroup : ICollection<Telemetry>
    {
        private ICollection<Telemetry> _collection = new List<Telemetry>();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public void Add(Telemetry item)
        {
            if (!this.Any(t => t.Data.UserId == item.Data.UserId))
                _collection.Add(item);
            else _collection.FirstOrDefault(t => t.Data.UserId == item.Data.UserId).Append(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Telemetry item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Telemetry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Telemetry> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool Remove(Telemetry item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    public class Telemetry
    {
        public int Type { get; set; }
        public TelemetryData Data { get; set; }

        public void Append(Telemetry telemetry)
        {
            if(string.Equals(telemetry.Data.UserId, this.Data.UserId, StringComparison.CurrentCulture))
            {
                foreach(APoint apoint in telemetry.Data.APoints)
                {
                    APoint res = this.Data.APoints.FirstOrDefault(ap => string.Equals(ap.Uid, apoint.Uid));
                    if(res == null)
                    {
                        this.Data.APoints.Add(apoint);
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
                Type = 1,
                Data = new TelemetryData
                {
                    UserId = userId,
                    APoints = new List<APoint>()
                }
            };
        }

        public void NewBeacon(string mac, int rssi, string apointUid, DateTime time)
        {
            var apoint = Data.APoints.FirstOrDefault(ap => ap.Uid == apointUid);
            if(apoint == null)
            {
                apoint = APoint.FromParse(apointUid);
                Data.APoints.Add(apoint);
            }

            var beacon = apoint.Beacons.FirstOrDefault(b => b.Mac == mac);
            if(beacon == null)
            {
                beacon = SingleBeaconTelemetry.FromParse(mac);
                apoint.Beacons.Add(beacon);
            }
            var rssivalue = beacon.Values.FirstOrDefault(v => v.Time == time);
            if(rssivalue == null)
            {
                rssivalue = new RssiValue { Rssi = rssi, Time = time };
            }
        }
    }

    public class TelemetryData
    {
        public string UserId { get; set; }

        public IList<APoint> APoints { get; set; }
    }

    public class APoint
    {
        public string Uid { get; set; }
        public IList<SingleBeaconTelemetry> Beacons { get; set; }
        public static APoint FromParse(string uid)
        {
            return new APoint
            {
                Uid = uid,
                Beacons = new List<SingleBeaconTelemetry>()
            };
        }
        public void Append(APoint apoint)
        {
            if(string.Equals(this.Uid, apoint.Uid, StringComparison.CurrentCultureIgnoreCase))
            {
                foreach(var beacon in apoint.Beacons)
                {
                    SingleBeaconTelemetry res = this.Beacons.FirstOrDefault(b => string.Equals(b.Mac, beacon.Mac, StringComparison.CurrentCultureIgnoreCase));
                    if(res == null)
                    {
                        this.Beacons.Add(beacon);
                    }
                    else
                    {
                        res.Append(beacon);
                    }
                }
            }
        }
    }

    public class SingleBeaconTelemetry
    {
        public string Mac { get; set; }
        public IList<RssiValue> Values { get; set; }

        public static SingleBeaconTelemetry FromParse(string mac)
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
