using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Trigger
{
    public class Ranger : IRanger, ITriggerEvents
    {
        #region Variables
        internal int slideAverageCount = 5;
        internal List<IBeaconBody> _firstLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _secondLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _helpBeacons { get; private set; } = new List<IBeaconBody>();
        private string _userUid { get; set; }
        internal AccessPoint apoint;

        private BeaconInfoGroup _firstLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _secondLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _helpLineInfo = new BeaconInfoGroup();
        private Beacon _lastBeacon;
        private AppearStatus _status = AppearStatus.Unknown;

        private TimeSpan timeOffset = new TimeSpan(0, 0, 2);
        #endregion

        private AppearStatus Status
        {
            get => _status;
            set
            {
                if (_status != AppearStatus.Unknown && _status != value)
                {
                    if (value == AppearStatus.Inside)
                        OnEnter?.Invoke(this, new TriggerEventArgs(apoint, _lastBeacon.DateTime, _userUid));
                    else
                        OnExit?.Invoke(this, new TriggerEventArgs(apoint, _lastBeacon.DateTime, _userUid));
                }

                _status = value;
            }
        }

        public event EventHandler<TriggerEventArgs> OnEnter;
        public event EventHandler<TriggerEventArgs> OnExit;

        #region Methods
        public void CheckTelemetry(Telemetry telemetry)
        {
            _userUid = telemetry.UserId;

            //var beacons = telemetry?.Data.APoints
            //    .FirstOrDefault(p => p.Uid == apoint.Uid).Beacons;

            IEnumerable<Beacon> commonList = telemetry[apoint.Uid].Beacons
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
            }

            commonList = null;
            Flush();
        }

        private void Flush()
        {
            _status = AppearStatus.Unknown;
            _userUid = "";
        }

        private void ResetSlideRssi(BeaconInfoGroup group, Beacon beacon)
        {
            foreach (var g in group)
            {
               if ((beacon.DateTime - g.LastRssiTime) >= timeOffset)
                    g.ResetSlideAverageRssi();
            }
        }

        /// <summary>
        /// Check slide average by RSSI
        /// </summary>
        /// <param name="apoint"></param>
        /// <param name="beacon"></param>
        private void CheckSlideAverage(AccessPoint apoint, Beacon beacon)
        {
            _lastBeacon = beacon;

            if (_secondLineInfo > BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo))
                Status = AppearStatus.Inside;
            else if (_firstLineInfo > _secondLineInfo)
                Status = AppearStatus.Outside;
        }

        private void RefreshBeaconInfoGroup(Beacon beacon)
        {
            Action<IList<IBeaconBody>, BeaconInfoGroup> CheckBeacon = (line, group) =>
            {
                if (line.Any(b => string.Equals(b.Mac, beacon.Mac, StringComparison.InvariantCultureIgnoreCase)))
                {
                    var res = group.FirstOrDefault(b => string.Equals(b.MacAddress, beacon.Mac, StringComparison.InvariantCultureIgnoreCase));
                    if (res == null)
                    {
                        res = new BeaconInfo(beacon.Mac);
                        res.SlideAverageCount = slideAverageCount;
                        group.Add(res);
                    }
                    res.SetLastRssi(beacon.Rssi, beacon.DateTime);

                    return;
                }
            };

            CheckBeacon(_firstLineBeacons, _firstLineInfo);
            CheckBeacon(_secondLineBeacons, _secondLineInfo);
            CheckBeacon(_helpBeacons, _helpLineInfo);

            ResetSlideRssi(_firstLineInfo, beacon);
            ResetSlideRssi(_secondLineInfo, beacon);
            ResetSlideRssi(_helpLineInfo, beacon);
        }

        public bool IsObsolete()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}