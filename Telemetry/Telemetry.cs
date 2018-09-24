using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trigger.Telemetry
{
    public class Telemetry// : Trigger.Telemetry.IBeaconsTelemetry
    {
        public int Type { get; set; }
        public TelemetryData Data { get; set; }

        public void Append(Telemetry telemetry)
        {
            //if (telemetry == null) return;
            if(this.Data?.UserId == telemetry.Data?.UserId)
            {
                foreach(var ap in telemetry.Data.APoints)
                {
                    APoint ap_res = this.Data.APoints.FirstOrDefault(p => p.Uid == ap.Uid);
                    if(ap_res == null)
                    {
                        ap_res = ap;
                        this.Data.APoints.Add(ap_res);
                    }
                    else
                    {
                        foreach(var beacon in ap.Beacons)
                        {
                            SingleBeaconTelemetry beac_res = ap_res.Beacons.FirstOrDefault(b => b.Mac == beacon.Mac);
                            if(beac_res == null)
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

        public Telemetry EmptyForUser(string userId)
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

    public class TelemetryData// : ITelemetryData
    {
        public string UserId { get; set; }

        public IList<APoint> APoints { get; set; }
    }

    public class APoint// : IAccessPoint
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

    public class SingleBeaconTelemetry// : Trigger.Telemetry.ISingleBeaconValue
    {
        public string Mac { get; set; }
        public IList<RssiValue> Values { get; set; }

        public static SingleBeaconTelemetry FromParse(string mac)
        {
            return new SingleBeaconTelemetry
            {
                Mac = mac,
                Values = new List<RssiValue>()
            }
        }
    }

    public class RssiValue// : IRssiValue
    {
        public int Rssi { get; set; }
        public DateTime Time { get; set; }
    }
}
