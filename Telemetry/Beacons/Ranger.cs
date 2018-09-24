using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SCAppLibrary.Android.Beacons;

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


    public class Ranger : ITriggerCallback
    {
        public string Uid { get; private set; }
        public List<IBeaconBody> FirstLineBeacons { get; private set; }
        public List<IBeaconBody> SecondLineBeacons { get; private set; }
        public List<IBeaconBody> HelpBeacons { get; private set; }
        public bool Inside { get; private set; }

        private List<BeaconInfo> foundBeacFirstLine;
        private List<BeaconInfo> foundBeacSecondLine;
        private List<BeaconInfo> foundBeacHelpLine;
        private ITriggerCallback Callback;
        private List<Beacon> commonList;

        public APoint apoint;
        private int slideAverageCount = 5;

        private Ranger()
        {
            FirstLineBeacons = new List<IBeaconBody>();
            SecondLineBeacons = new List<IBeaconBody>();
            HelpBeacons = new List<IBeaconBody>();
            foundBeacFirstLine = new List<BeaconInfo>();
            foundBeacSecondLine = new List<BeaconInfo>();
            foundBeacHelpLine = new List<BeaconInfo>();

            Callback = this;
        }

        public event EventHandler<TriggerEventArgs> Enter;
        public event EventHandler<TriggerEventArgs> Exit;

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
                Callback?.OnEnter(apoint, beacon.DateTime);
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
                Callback?.OnExit(apoint, beacon.DateTime);
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
                //CheckEnter();
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
                //CheckEnter();
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
                //CheckEnter();
                return;
            }
        }

        private void ResetAllSlideAverageRssi(IList<BeaconInfo> beacons)
        {
            for(int i = 0; i< beacons.Count; i++)
            {
                beacons[i].ResetSlideAverageRssi();
            }
        }

        public void OnEnter(APoint apoint, DateTime time)
        {
            Enter(this, new TriggerEventArgs(apoint, time));
        }

        public void OnExit(APoint apoint, DateTime time)
        {
            Exit(this, new TriggerEventArgs(apoint, time));
        }

       

        public class Builder
        {
            private Ranger ranger;

            public Builder()
            {
                ranger = new Ranger();
            }

            //public Builder SetCallback(ITriggerCallback callback)
            //{
            //    this.ranger.Callback = callback;
            //    return this;
            //}

            public Builder AddFirstLineBeacon(IBeaconBody beacon)
            {
                ranger.FirstLineBeacons.Add(beacon);
                return this;
            }

            public Builder AddSecondLineBeacon(IBeaconBody beacon)
            {
                ranger.SecondLineBeacons.Add(beacon);
                return this;
            }

            public Builder AddHelpBeacon(IBeaconBody beacon)
            {
                ranger.HelpBeacons.Add(beacon);
                return this;
            }

            public Builder SetCalcSlideAverageCount(int count)
            {
                ranger.slideAverageCount = count;
                return this;
            }

            public Ranger Build()
            {
                return this.ranger;
            }
        }


    }
}
