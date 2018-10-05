using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Trigger
{
    public class Ranger : IRanger
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
        private BeaconItem _lastBeacon;
        private AppearStatus _status = AppearStatus.Unknown;

        private TimeSpan timeOffset = new TimeSpan(0, 0, 2);
        #endregion

        internal void ChangeStatus(AppearStatus value)
        {
            if (_status != AppearStatus.Unknown && _status != value)
                OnEvent?.Invoke(this, new TriggerEventArgs
                {
                    AccessPointUid = apoint.Uid,
                    DateTime = _lastBeacon.Time,
                    UserId = _userUid,
                    Type = (value == AppearStatus.Inside ? TriggerEventType.Enter : TriggerEventType.Exit)
                });

            _status = value;
        }

        public event EventHandler<TriggerEventArgs> OnEvent;

        #region Methods
        public void CheckTelemetry(Telemetry telemetry)
        {
            _userUid = telemetry.UserId;

            var data = telemetry[apoint.Uid].Beacons
                .SelectMany(b => b.Select(bi => new { mac = b.Mac, Item = bi })).OrderBy(x => x.Item.Time);

            foreach (var beacon in data)
            {
                RefreshBeaconInfoGroup(beacon.mac, beacon.Item);

                //  CheckSlideAverage(apoint, beacon.LastItem);
            }

            Flush();
        }

        private void Flush()
        {
            _status = AppearStatus.Unknown;
            _userUid = "";
        }

        private void ResetSlideRssi(BeaconInfoGroup group, DateTime time)
        {
            foreach (var g in group)
            {
                if ((time - g.LastRssiTime) >= timeOffset)
                    g.ResetSlideAverageRssi();
            }
        }

        /// <summary>
        /// Check slide average by RSSI
        /// </summary>
        /// <param name="apoint"></param>
        /// <param name="beacon"></param>
        private void CheckSlideAverage(AccessPoint apoint, BeaconItem beacon)
        {
            _lastBeacon = beacon;

            if (_secondLineInfo > BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo))
                ChangeStatus(AppearStatus.Inside);
            else if (_firstLineInfo > _secondLineInfo)
                ChangeStatus(AppearStatus.Outside);
        }

        private void RefreshBeaconInfoGroup(string macAddr, BeaconItem beacon)
        {
            bool flag = false;

            Action<IList<IBeaconBody>, BeaconInfoGroup> CheckBeacon = (line, group) =>
            {
                if (flag)
                    return;

                if (line.Any(b => string.Equals(b.Mac, macAddr, StringComparison.InvariantCultureIgnoreCase)))
                {
                    flag = true;

                    var res = group.FirstOrDefault(b => string.Equals(b.MacAddress, macAddr, StringComparison.InvariantCultureIgnoreCase));
                    if (res == null)
                    {
                        res = new BeaconInfo(macAddr);
                        res.SlideAverageCount = slideAverageCount;
                        group.Add(res);
                    }
                    res.SetLastRssi(beacon);

                    ResetSlideRssi(group, beacon.Time);

                    return;
                }
            };

            CheckBeacon(_firstLineBeacons, _firstLineInfo);
            CheckBeacon(_secondLineBeacons, _secondLineInfo);
            CheckBeacon(_helpBeacons, _helpLineInfo);

            //ResetSlideRssi(_firstLineInfo, beacon);
            //ResetSlideRssi(_secondLineInfo, beacon);
            //ResetSlideRssi(_helpLineInfo, beacon);
        }

        public bool IsObsolete()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}