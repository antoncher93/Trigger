using System;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Logging;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Trigger
{
    public class Ranger : IRanger
    {
        private IDisposable unsubscriber;

        #region Variables
        internal int slideAverageCount = 5;
        internal int rssiBarier = 0;

        internal List<IBeaconBody> _firstLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _secondLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _helpBeacons { get; private set; } = new List<IBeaconBody>();
        private string _userUid { get; set; }
        internal AccessPointData apoint;
        internal int _actualSignalPeriod = 1000;

        private BeaconInfoGroup _firstLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _secondLineInfo = new BeaconInfoGroup();
        private BeaconInfoGroup _helpLineInfo = new BeaconInfoGroup();

        private AppearStatus _status = AppearStatus.Unknown;
        private DateTime _currentTime;
        internal ILogger _logger;

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
                    AccessPointUid = apoint.AccessPointUid,
                    Timespan = _currentTime,
                    UserId = _userUid,
                    Type = (value == AppearStatus.Inside ? TriggerEventType.Enter : TriggerEventType.Exit)
                });

            }
            _status = value;
        }

        public event EventHandler<TriggerEventArgs> OnEvent;

        #region Methods
        private void ProduceEvent(Telemetry telemetry)
        {
            if (!telemetry.Contains(apoint.AccessPointUid))
                return;

            _userUid = telemetry.UserId;

            BeaconItem prevSignal = BeaconItem.Default;

            var data = telemetry[apoint.AccessPointUid].Beacons
                .SelectMany(beacon => beacon.Select(beaconItem => new { mac = beacon.Address, Item = beaconItem })).OrderBy(x => x.Item.Time);

            if (!data.Any())
                return;

            // refactoring ----
            IEnumerable<DateTime> checkPoints = data.Select(d => d.Item.Time).Distinct();

            foreach (DateTime date in checkPoints)
            {
                foreach (var beaconSignal in data.Where(beaconSignal => beaconSignal.Item.Time == date))
                    RefreshBeaconInfoGroup(beaconSignal.mac, beaconSignal.Item);

                CheckActualRssi(date);
            }
            // ----------------

            /* Obsolete
            var current = data.First();

            while (true)
            {
                var oneTime = data.Where(v => v.Item.Time == current.Item.Time);
                foreach (var beacon in oneTime)
                {
                    RefreshBeaconInfoGroup(beacon.mac, beacon.Item);
                }

                CheckActualRssi(current.Item.Time);

                current = data.FirstOrDefault(f => f.Item.Time > current.Item.Time);
                if (current == null)
                    break;
            }
            */

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
        /// <param name="beacon"></param>
        private void CheckSlideAverage(BeaconItem beacon)
        {
            Update(beacon.Time);

            _currentTime = beacon.Time;

            if ((_secondLineInfo - BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo)) >= rssiBarier)
                ChangeStatus(AppearStatus.Inside);
            else if ((_firstLineInfo - _secondLineInfo) >= rssiBarier)
                ChangeStatus(AppearStatus.Outside);
        }

        private void CheckActualRssi(DateTime actualTime)
        {
            Update(actualTime);

            _currentTime = actualTime;

            if (_secondLineInfo > _firstLineInfo)
            {
                ChangeStatus(AppearStatus.Inside);
            }
            else if (_firstLineInfo > _secondLineInfo)
            {
                ChangeStatus(AppearStatus.Outside);
            }
        }

        private void RefreshBeaconInfoGroup(MacAddress macAddress, BeaconItem beacon)
        {
            bool flag = false;

            Action<IList<IBeaconBody>, BeaconInfoGroup> CheckBeacon = (line, group) =>
            {
                if (flag)
                    return;

                if (line.Any(b => b.Address == macAddress))
                {
                    flag = true;

                    var res = group.FirstOrDefault(b => b.Address == macAddress);
                    if (res == null)
                    {
                        res = _logger == null ?
                        new BeaconInfo(macAddress, _actualSignalPeriod)
                        : new BeaconInfo(macAddress, _actualSignalPeriod, _logger);

                        group.Add(res);
                    }
                    res.Add(beacon);
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
            return false;
            //  throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            this.Unsubscribe();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Telemetry value)
        {
            ProduceEvent(value);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public void Subscribe(IObservable<Telemetry> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }
        #endregion
    }
}
