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
        internal int rssiBarier = 0;
        internal int _signalLifePeriod
        {
            set
            {
                _firstLineInfo.TimeOffset = new TimeSpan(0, 0, 0, 0, value);
                _secondLineInfo.TimeOffset = new TimeSpan(0, 0, 0, 0, value);
                _helpLineInfo.TimeOffset = new TimeSpan(0, 0, 0, 0, value);
            }
        }
        internal List<IBeaconBody> _firstLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _secondLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _helpBeacons { get; private set; } = new List<IBeaconBody>();
        private string _userUid { get; set; }
        internal AccessPoint apoint;

        private BeaconInfoGroup _firstLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _secondLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _helpLineInfo = new BeaconInfoGroup();
        
        private AppearStatus _status = AppearStatus.Unknown;
        private TimeSpan timeOffset = new TimeSpan(0, 0, 2);
        
        private bool _anyLinesChanged = false;
        private DateTime _currentTime;
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
                    DateTime = _currentTime,
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

            BeaconItem prevSignal = BeaconItem.Default;

            var data = telemetry[apoint.Uid].Beacons
                .SelectMany(b => b.Select(bi => new { mac = b.Mac, Item = bi })).OrderBy(x => x.Item.Time);

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
                    CheckSlideAverage(fir.Item);
                    _anyLinesChanged = false;
                }

                fir = data.FirstOrDefault(f => f.Item.Time > fir.Item.Time);

                if (fir == null) break;
            }
            Flush();
        }

        public void CheckTelemetryByActualRssi(Telemetry telemetry)
        {
            _userUid = telemetry.UserId;

            BeaconItem prevSignal = BeaconItem.Default;

            var data = telemetry[apoint.Uid].Beacons
                .SelectMany(b => b.Select(bi => new { mac = b.Mac, Item = bi })).OrderBy(x => x.Item.Time);

            var any = data.Any();
            var fir = data.First();
            while (any)
            {
                
                var oneTime = data.Where(v => v.Item.Time == fir.Item.Time);
                foreach (var beacon in oneTime)
                {
                    RefreshBeaconInfoGroup(beacon.mac, beacon.Item);
                }

                CheckActualRssi(fir.Item.Time);

                fir = data.FirstOrDefault(f => f.Item.Time > fir.Item.Time);
                if (fir == null)
                    break;
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

        private void CheckSlideAverage(BeaconItem beacon)
        {
            Update(beacon.Time);

            _currentTime = beacon.Time;

            if ((_secondLineInfo - BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo))>=rssiBarier)
                ChangeStatus(AppearStatus.Inside);
            else if ((_firstLineInfo - _secondLineInfo)>=rssiBarier)
                ChangeStatus(AppearStatus.Outside);
        }


        private void CheckActualRssi(DateTime actualTime)
        {
            Update(actualTime);

            _currentTime = actualTime;

            if(_secondLineInfo.MaxAverRssi > _firstLineInfo.MaxAverRssi)
            {
                ChangeStatus(AppearStatus.Inside);
            }
            else if(_firstLineInfo.MaxAverRssi > _secondLineInfo.MaxAverRssi)
            {
                ChangeStatus(AppearStatus.Outside);
            }
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
                        group.Add(res);
                    }
                    res.SetLastRssi(beacon);
                    return;
                }
            };

            CheckBeacon(_firstLineBeacons, _firstLineInfo);
            CheckBeacon(_secondLineBeacons, _secondLineInfo);
            CheckBeacon(_helpBeacons, _helpLineInfo);
        }

        private void Update(DateTime actual_time) // проверить всемя последних сигналов от линий
        {
            _firstLineInfo.Update(actual_time);
            _secondLineInfo.Update(actual_time);
            _helpLineInfo.Update(actual_time);
        }

        public bool IsObsolete()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}