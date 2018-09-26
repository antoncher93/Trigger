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
            if (Data?.UserId == telemetry.Data?.UserId)
            {
                foreach(var ap in telemetry.Data.APoints)
                {
                    APoint ap_res = Data.APoints.FirstOrDefault(p => p.Uid == ap.Uid);
                    if (ap_res == null)
                    {
                        ap_res = ap;
                        Data.APoints.Add(ap_res);
                    }
                    else
                    {
                        foreach (var beacon in ap.Beacons)
                        {
                            SingleBeaconTelemetry beac_res = ap_res.Beacons.FirstOrDefault(b => b.Mac == beacon.Mac);
                            if (beac_res == null)
                            {
                                beac_res = beacon;
                                ap_res.Beacons.Add(beac_res);
                            }
                            else
                            {
                                foreach(var value in beacon.Values)
                                {
                                    RssiValue val_res = beac_res.Values.FirstOrDefault(v => v.Time == value.Time);
                                    if(val_res == null)
                                    {
                                        val_res = value;
                                        beac_res.Values.Add(val_res);
                                    }
                                }
                            }
                        }
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
    }

    public class RssiValue
    {
        public int Rssi { get; set; }
        public DateTime Time { get; set; }
    }
}
