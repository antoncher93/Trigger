using System;
using System.Collections.Generic;
using System.Linq;

namespace Trigger.Telemetry.Beacons
{
    public class TriggerEventArgs : EventArgs
    {
        public TriggerEventArgs(APoint apoint, DateTime time)
        {
            APoint = apoint;
            DateTime = time;
        }
        public APoint APoint { get; private set; }
        public DateTime DateTime { get; private set; }
    }


    public class Ranger
    {
        public List<IBeaconBody> FirstLineBeacons { get; private set; }
        public List<IBeaconBody> SecondLineBeacons { get; private set; }
        public List<IBeaconBody> HelpBeacons { get; private set; }
        public bool Inside { get; private set; }

        private List<BeaconInfo> foundBeacFirstLine;
        private List<BeaconInfo> foundBeacSecondLine;
        private List<BeaconInfo> foundBeacHelpLine;
        
        private List<Beacon> commonList;

        private APoint apoint;
        private int slideAverageCount = 5;

        public IList<TimeSpan?> PeakDistances { get; private set; }

        public Ranger()
        {
            FirstLineBeacons = new List<IBeaconBody>();
            SecondLineBeacons = new List<IBeaconBody>();
            HelpBeacons = new List<IBeaconBody>();
            foundBeacFirstLine = new List<BeaconInfo>();
            foundBeacSecondLine = new List<BeaconInfo>();
            foundBeacHelpLine = new List<BeaconInfo>();

            PeakDistances = new List<TimeSpan?>();
        }

        public event EventHandler<TriggerEventArgs> Enter;
        public event EventHandler<TriggerEventArgs> Exit;

        public event EventHandler<TriggerEventArgs> EnterByPeaks;

        public void CheckTelemetry(Telemetry telemetry)
        {
            var res = telemetry.Data.APoints.FirstOrDefault(p => p.Uid == this.apoint.Uid);

            if (res == null) return;

            var beacons = res.Beacons;

            commonList = beacons.SelectMany(s => s.Values.Select(v => new Beacon { Mac = s.Mac, Rssi = v.Rssi, DateTime = v.Time })).ToList();
            commonList = commonList.OrderBy(s => s.DateTime).ToList();

            for (int i = 0; i < commonList.Count; i++)
            {
                RefreshBeaconInfo(commonList[i]);

                if (Inside)
                {
                    CheckExit(apoint, commonList[i]);
                }
                else
                {
                    CheckEnter(apoint, commonList[i]);
                }

                commonList.RemoveAt(i);
                i--;
            }

            CalcPeakDistances();
        }

        public void CheckTelemetry(string str)
        {
            Telemetry telemetry = Newtonsoft.Json.JsonConvert.DeserializeObject<Telemetry>(str);
            CheckTelemetry(telemetry);
        }

        private void CheckEnter(APoint apoint, Beacon beacon)
        {
            if (Inside) return;

            var max_1 = foundBeacFirstLine.Union(foundBeacHelpLine).OrderByDescending(b => b.AverageRssi).FirstOrDefault();
            var max_2 = foundBeacSecondLine.OrderByDescending(b => b.SlideAverageRssi).FirstOrDefault();

            if (max_2?.SlideAverageRssi >= max_1?.AverageRssi)
            {
                Inside = true;
                Enter?.Invoke(this, new TriggerEventArgs(apoint, beacon.DateTime));
                ResetAllSlideAverageRssi(foundBeacFirstLine);
            }
        }

        private void CheckExit(APoint apoint, Beacon beacon)
        {
            if (!Inside) return;

            BeaconInfo max_1 = foundBeacSecondLine.OrderByDescending(b => b.AverageRssi).FirstOrDefault();
            BeaconInfo max_2 = foundBeacFirstLine.OrderByDescending(b => b.SlideAverageRssi).FirstOrDefault();

            if (max_2?.SlideAverageRssi >= max_1?.AverageRssi)
            {
                Inside = false;
                Exit?.Invoke(this, new TriggerEventArgs(apoint, beacon.DateTime));
                ResetAllSlideAverageRssi(foundBeacSecondLine);
            }
        }

        private void RefreshBeaconInfo(Beacon beacon)
        {
            if (FirstLineBeacons.Any(b => b.Mac == beacon.Mac))
            {
                var res = foundBeacFirstLine.FirstOrDefault(beac => beac.MacAddress == beacon.Mac);
                if (res == null)
                {
                    res = new BeaconInfo(beacon.Mac);
                    res.SlideAverageCount = slideAverageCount;
                    foundBeacFirstLine.Add(res);
                }
                res.SetLastRssi(beacon.Rssi, beacon.DateTime);

                return;
            }

            if (SecondLineBeacons.Any(b => b.Mac == beacon.Mac))
            {
                var res = foundBeacSecondLine.FirstOrDefault(beac => beac.MacAddress == beacon.Mac);
                if (res == null)
                {
                    res = new BeaconInfo(beacon.Mac)
                    {
                        SlideAverageCount = slideAverageCount
                    };
                    foundBeacSecondLine.Add(res);
                }
                res.SetLastRssi(beacon.Rssi, beacon.DateTime);

                return;
            }

            if (HelpBeacons.Any(b => b.Mac == beacon.Mac))
            {
                var res = foundBeacHelpLine.FirstOrDefault(beac => beac.MacAddress == beacon.Mac);
                if (res == null)
                {
                    res = new BeaconInfo(beacon.Mac)
                    {
                        SlideAverageCount = slideAverageCount
                    };
                    foundBeacHelpLine.Add(res);
                }
                res.SetLastRssi(beacon.Rssi, beacon.DateTime);

                return;
            }
        }

        private void CheckEnterByPeaks()
        {
            var beacon1 = foundBeacFirstLine.FirstOrDefault(b => b.Peak != null);
            if (beacon1 == null) return;
            var beacon2 = foundBeacSecondLine.FirstOrDefault(b => b.Peak != null);
            if (beacon2 == null) return;

            if((beacon1.Peak.Time - beacon2.Peak.Time).TotalSeconds >= 3)
            {
                EnterByPeaks(this, new TriggerEventArgs(apoint, beacon2.Peak.Time));
            }

        }

        private void CalcPeakDistances()
        {
            int i = 0;
            bool next = true;
            while(next)
            {
                if(foundBeacFirstLine.Count > i && foundBeacSecondLine.Count > i)
                {
                    PeakDistances.Add(foundBeacFirstLine[i].Peak?.Time - foundBeacSecondLine[i].Peak?.Time);
                    i++;
                }
                else
                {
                    next = false;
                }
            }
        }

        private void ResetAllSlideAverageRssi(IList<BeaconInfo> beacons)
        {
            for(int i = 0; i< beacons.Count; i++)
            {
                beacons[i].ResetSlideAverageRssi();
            }
        }

        public void SetSlideAverageCount(int _slideAverageCount)
            => slideAverageCount = _slideAverageCount;
        public void SetAPoint(APoint _apoint)
            => apoint = _apoint;
    }
}
