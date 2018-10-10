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
        private int _barier = 0;
        private bool _anyLinesChanged = false;
        private DateTime _lastIventTime = DateTime.MinValue;
        private TimeSpan iventTimeOffset = new TimeSpan(0, 0, 2);
        #endregion

        internal void ChangeStatus(AppearStatus value)
        {
            if (
                _status != AppearStatus.Unknown && 
                _status != value
                )
            {
                OnEvent?.Invoke(this, new TriggerEventArgs
                {
                    AccessPointUid = apoint.Uid,
                    DateTime = _lastBeacon.Time,
                    UserId = _userUid,
                    Type = (value == AppearStatus.Inside ? TriggerEventType.Enter : TriggerEventType.Exit)
                });
                
            }
            _status = value;

        }

        public event EventHandler<TriggerEventArgs> OnEvent;

        #region Methods
        public void CheckTelemetry(Telemetry telemetry)
        {
            _userUid = telemetry.UserId;

            //заглушка 
            _barier = 0;

            BeaconItem prevSignal = BeaconItem.Default;

            var data = telemetry[apoint.Uid].Beacons
                .SelectMany(b => b.Select(bi => new { mac = b.Mac, Item = bi })).OrderBy(x => x.Item.Time).ToList();

            var any = data.Any();
            var fir = data.First();
            while (any)
            {
                var oneTime = data.Where(v => v.Item.Time == fir.Item.Time);
                
                foreach(var beacon in oneTime)
                {
                    RefreshBeaconInfoGroup(beacon.mac, beacon.Item);
                }
                if(_anyLinesChanged)
                {
                    CheckSlideAverage(apoint, fir.Item);
                    _anyLinesChanged = false;
                }

                fir = data.FirstOrDefault(f => f.Item.Time > fir.Item.Time);

                if (fir == null) break;
            }
            Flush();
        }

        private void Flush()
        {
            _status = AppearStatus.Unknown;
            _userUid = "";  
        }

        /// <summary>
        /// Check slide average by RSSI
        /// </summary>
        /// <param name="apoint"></param>
        /// <param name="beacon"></param>
        /// 

        private void CheckSlideAverage(AccessPoint apoint, BeaconItem beacon)
        {
            UpdateLinesRss(beacon.Time);

            _lastBeacon = beacon;

            if ((_secondLineInfo - BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo))>_barier)
                ChangeStatus(AppearStatus.Inside);
            else if ((_firstLineInfo - _secondLineInfo)>_barier)
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
                        res = new BeaconInfo(macAddr) { SlideAverageCount = slideAverageCount };
                        group.Add(res);
                    }
                    res.SetLastRssi(beacon);

                    //ResetSlideRssi(group, beacon.Time);

                    _anyLinesChanged = true;
                    return;
                }
            };

            CheckBeacon(_firstLineBeacons, _firstLineInfo);
            CheckBeacon(_secondLineBeacons, _secondLineInfo);
            CheckBeacon(_helpBeacons, _helpLineInfo);
        }

        private void UpdateLinesRss(DateTime actual_time) // проверить всемя последних сигналов от линий
        {
            _firstLineInfo.UpdateSlideAverageRssi(actual_time);
            _secondLineInfo.UpdateSlideAverageRssi(actual_time);
            _helpLineInfo.UpdateSlideAverageRssi(actual_time);
        }

        public bool IsObsolete()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}