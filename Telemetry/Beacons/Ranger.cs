using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.Telemetry.Interfaces;

namespace Trigger.Telemetry.Beacons
{
    public class Ranger : IRangerEvents
    {
        #region Variables
        private int slideAverageCount = 5;

        public List<IBeaconBody> FirstLineBeacons { get; private set; } = new List<IBeaconBody>();
        public List<IBeaconBody> SecondLineBeacons { get; private set; } = new List<IBeaconBody>();
        public List<IBeaconBody> HelpBeacons { get; private set; } = new List<IBeaconBody>();

        private BeaconInfoGroup FirstLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup SecondLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup HelpLineInfo = new BeaconInfoGroup();

        private IList<Beacon> commonList = new List<Beacon>();

        private APoint apoint;

        public IList<TimeSpan?> PeakDistances { get; private set; } = new List<TimeSpan?>();
        private Beacon lastbeacon;

        private AppearStatus status = AppearStatus.Unknown;

        private string UserUid { get; set; }
        #endregion

        private AppearStatus Status
        {
            get => status;
            set
            {
                if (status != AppearStatus.Unknown && status != value)
                {
                    if (value == AppearStatus.Inside)
                    {
                        Enter?.Invoke(this, new TriggerEventArgs(apoint, lastbeacon.DateTime, UserUid));
                    }
                    else
                    {
                        Exit?.Invoke(this, new TriggerEventArgs(apoint, lastbeacon.DateTime, UserUid));
                    }
                }

                status = value;
            }
        }

        public event EventHandler<TriggerEventArgs> Enter;
        public event EventHandler<TriggerEventArgs> Exit;
        public event EventHandler<TriggerEventArgs> EnterByPeaks;

        public void CheckTelemetry(Telemetry telemetry)
        {
            if (telemetry == null)
                return;

            var res = telemetry.Data.APoints.FirstOrDefault(p => p.Uid == apoint.Uid);

            if (res == null)
                return;

            var beacons = res.Beacons;

            commonList = beacons
                .SelectMany(s => 
                    s.Values.Select(v => 
                        new Beacon {
                            Mac = s.Mac,
                            Rssi = v.Rssi,
                            DateTime = v.Time
                        }))
                .OrderBy(s => s.DateTime)
                .ToList();

            foreach (var beacon in commonList)
            {
                RefreshBeaconInfoGroup(beacon);
                CheckSlideAverage(apoint, beacon);

                commonList.Remove(beacon);
            }
        }

        private void ResetSlideRssi(BeaconInfoGroup group, Beacon beacon, TimeSpan? timeoffset)
        {
            Parallel.ForEach(group, (g) => {
                if ((beacon.DateTime - g.LastRssiTime) >= timeoffset)
                {
                    g.ResetSlideAverageRssi();
                }
            });
        }

        /// <summary>
        /// Check slide average by RSSI
        /// </summary>
        /// <param name="apoint"></param>
        /// <param name="beacon"></param>
        public void CheckSlideAverage(APoint apoint, Beacon beacon)
        {
            lastbeacon = beacon;

            if (SecondLineInfo > BeaconInfoGroup.Max(FirstLineInfo, HelpLineInfo))
                Status = AppearStatus.Inside;
            else if (FirstLineInfo > SecondLineInfo)
                Status = AppearStatus.Outside;

        }

        private void RefreshBeaconInfoGroup(Beacon beacon)
        {
            Action<Beacon, IList<IBeaconBody>, BeaconInfoGroup> CheckBeacon = (beac, line, group) =>
            {
                if (line.Any(b => b.Mac == beac.Mac))
                {

                    var res = group.FirstOrDefault(b => b.MacAddress == beacon.Mac);
                    if (res == null)
                    {
                        res = new BeaconInfo(beac.Mac);
                        res.SlideAverageCount = slideAverageCount;
                        group.Add(res);
                    }
                    res.SetLastRssi(beac.Rssi, beac.DateTime);

                    return;
                }
            };

            CheckBeacon(beacon, FirstLineBeacons, FirstLineInfo);
            CheckBeacon(beacon, SecondLineBeacons, SecondLineInfo);
            CheckBeacon(beacon, HelpBeacons, HelpLineInfo);

            TimeSpan? timeoffset = new TimeSpan?(new TimeSpan(0, 0, 2));

            ResetSlideRssi(FirstLineInfo, beacon, timeoffset);
            ResetSlideRssi(SecondLineInfo, beacon, timeoffset);
            ResetSlideRssi(HelpLineInfo, beacon, timeoffset);
        }

        public void SetSlideAverageCount(int _slideAverageCount)
            => slideAverageCount = _slideAverageCount;

        public void SetAPoint(APoint _apoint)
            => apoint = _apoint;
    }
}