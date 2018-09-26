using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.Interfaces;

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


    public class Ranger : IRangerEvents//, IRangerConsumer
    {
        #region Events
        public event EventHandler<TriggerEventArgs> Enter;
        public event EventHandler<TriggerEventArgs> Exit;
        public event EventHandler<TriggerEventArgs> EnterByPeaks;
        #endregion

        public IList<IBeaconBody> FirstLineBeacons { get; private set; }
        public IList<IBeaconBody> SecondLineBeacons { get; private set; }
        public IList<IBeaconBody> HelpBeacons { get; private set; }
        

        private IList<BeaconInfo> foundBeacFirstLine;
        private IList<BeaconInfo> foundBeacSecondLine;
        private IList<BeaconInfo> foundBeacHelpLine;
       
        private APoint APoint;

        private bool Inside = false;
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

        public void CheckTelemetry(Telemetry telemetry)
        {
            var res = telemetry.Data.APoints.FirstOrDefault(p => p.Uid == APoint.Uid);

            if (res == null) return;

            var beacons = res.Beacons;

            IOrderedEnumerable<Beacon> commonList = 
                beacons.SelectMany(s => s.Values.Select(v => 
                    new Beacon { Mac = s.Mac, Rssi = v.Rssi, DateTime = v.Time }))
                    .OrderBy(s => s.DateTime);

            foreach (var b in commonList)
            {
                RefreshBeaconInfo(b);
                if (Inside)
                    CheckExit(APoint, b);
                else
                    CheckEnter(APoint,b);
            }

            commonList = null;
        }

        public void CheckTelemetry(string str)
        {
            Telemetry telemetry = JsonConvert.DeserializeObject<Telemetry>(str);

            CheckTelemetry(telemetry);

            CalcPeakDistances();
        }

        private void CheckEnter(APoint apoint, Beacon beacon)
        {
            if (Inside) return;

            BeaconInfo max_1 = foundBeacFirstLine.Concat(foundBeacHelpLine).OrderByDescending(b => b.AverageRssi).FirstOrDefault();
            BeaconInfo max_2 = foundBeacSecondLine.OrderByDescending(b => b.SlideAverageRssi).FirstOrDefault();

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
            Action<IList<IBeaconBody>, IList<BeaconInfo>> PopulateBeacon = (line, foundLine) =>
                {
                    if (line.Any(b => string.Equals(b.Mac, beacon.Mac, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var res = foundLine.FirstOrDefault(beac => 
                            string.Equals(beac.MacAddress, beacon.Mac, StringComparison.InvariantCultureIgnoreCase));

                        if (res == null)
                        {
                            res = new BeaconInfo(beacon.Mac);
                            res.SlideAverageCount = slideAverageCount;
                            foundLine.Add(res);
                        }
                        res.SetLastRssi(beacon.Rssi, beacon.DateTime);

                        return;
                    }
                };

            PopulateBeacon(FirstLineBeacons, foundBeacFirstLine);
            PopulateBeacon(SecondLineBeacons, foundBeacSecondLine);
            PopulateBeacon(HelpBeacons, foundBeacHelpLine);
        }

        private void CheckEnterByPeaks()
        {
            var beacon1 = foundBeacFirstLine.FirstOrDefault(b => b.Peak != null);
            if (beacon1 == null) return;
            var beacon2 = foundBeacSecondLine.FirstOrDefault(b => b.Peak != null);
            if (beacon2 == null) return;

            if((beacon1.Peak.Time - beacon2.Peak.Time).TotalSeconds >= 3)
            {
                EnterByPeaks(this, new TriggerEventArgs(APoint, beacon2.Peak.Time));
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
            => Parallel.ForEach(beacons, (b) => b.ResetSlideAverageRssi());

        public void SetSlideAverageCount(int _slideAverageCount)
            => slideAverageCount = _slideAverageCount;

        public void SetAPoint(APoint _apoint)
            => APoint = _apoint;
    }
}