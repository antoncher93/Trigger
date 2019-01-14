using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Logging;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Rangers;
using Trigger.Signal;

namespace Trigger
{
    public class Ranger : TwoLineRanger
    {
        private IDisposable unsubscriber;
        private StringBuilder sb = new StringBuilder();
        #region Variables
        internal int slideAverageCount = 5;
        internal int rssiBarier = 0;

        private AppearStatus _status = AppearStatus.Unknown;

        internal TimeSpan StatusHolderPeriod { get; set; } = TimeSpan.FromSeconds(3);

        private StatusHolder _statusHolder { get; set; } = StatusHolder.Empty;

        #endregion

        internal void ChangeStatus(AppearStatus value, DateTime time)
        {
            if (
                _status != AppearStatus.Unknown &&
                _status != value
                )
            {
                var activity = value == AppearStatus.Inside ? TriggerEventType.Enter : TriggerEventType.Exit;
                report.Current.Event = activity.ToString();
                RaiseEvent(activity, time);
            }
            _status = value;
        }
        

        public override void OnNext(Telemetry value)
        {
            base.OnNext(value);

            //ProduceEvent(value);
            ProduceEvent2(value);
        }

        #region Methods

        private void ProduceEvent2(Telemetry telemetry)
        {
            report = new RangerReport();

            Telemetry accessible = telemetry[_firstLineBeacons.Union(_secondLineBeacons).Union(_helpBeacons)];

            if (!accessible.Any())
                return;

            _userUid = accessible.UserId;

            BeaconItem prevSignal = BeaconItem.Default;

            var data = accessible.SelectMany(beacon => beacon.Select(beaconItem => new { mac = beacon.Address, Item = beaconItem })).OrderBy(x => x.Item.Time);

            if (!data.Any())
                return;

            DateTime lastSignalTime = data.Select(d => d.Item.Time).OrderByDescending(t => t).FirstOrDefault();
            DateTime startPoint = data.Select(d => d.Item.Time).Distinct().FirstOrDefault();

            var period = TimeSpan.FromMilliseconds(_actualSignalPeriod);

            for (DateTime t = startPoint; t < (lastSignalTime + period); t += _timeStep)
            {
                var actuals = data.Where(b => b.Item.Time <= t && b.Item.Time > t - period);

                foreach(var beac in actuals.GroupBy(d => d.mac))
                {
                    double averRssi = beac.Select(b => b.Item.Rssi).Average();
                    BeaconItem item = new BeaconItem { Rssi = averRssi, Time = t };
                    Update(t);
                    RefreshBeaconInfoGroup(beac.Key, item);
                    
                }
                sb.Append($"{t.TimeOfDay.ToString()}$");

                CheckActualRssi(t);

            }

        }

        private void ProduceEvent(Telemetry telemetry)
        {
            report = new RangerReport();

            Telemetry accessible = telemetry[_firstLineBeacons.Union(_secondLineBeacons).Union(_helpBeacons)];

            if (!accessible.Any())
                return;

            _userUid = accessible.UserId;

            BeaconItem prevSignal = BeaconItem.Default;

            var data = accessible.SelectMany(beacon => beacon.Select(beaconItem => new { mac = beacon.Address, Item = beaconItem })).OrderBy(x => x.Item.Time);

            if (!data.Any())
                return;


            //DateTime lastSignalTime = data.Select(d => d.Item.Time).OrderByDescending(t => t).FirstOrDefault();
            //DateTime startPoint = data.Select(d => d.Item.Time).Distinct().FirstOrDefault();

            //var period = TimeSpan.FromMilliseconds(_actualSignalPeriod);

            //for (DateTime t = startPoint; t < (lastSignalTime + period); t+= period)
            //{

            //    var currents = data.Where(b => b.Item.Time <= t && b.Item.Time > t - period);

            //    currents.Select()

            //    foreach(var b in currents)
            //    {

            //        var s = new BeaconItem { Rssi = }

            //        RefreshBeaconInfoGroup(b.mac, )
            //    }
                
            //}


            // refactoring ----


            //IEnumerable<DateTime> checkPoints = data.Select(d => d.Item.Time).Distinct();

            //foreach (DateTime date in checkPoints)
            //{
            //    foreach (var beaconSignal in data.Where(beaconSignal => beaconSignal.Item.Time == date))
            //        RefreshBeaconInfoGroup(beaconSignal.mac, beaconSignal.Item);

                
            //    sb.Append($"{date.TimeOfDay.ToString()}$");

            //    CheckActualRssi(date);
            //}
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

        private void UpdateStatusHolder(DateTime time)
        {
            if(!_statusHolder.Equals(StatusHolder.Empty) &&
                time -_statusHolder.Time >= StatusHolderPeriod)
            {
                ChangeStatus(_statusHolder.Status, _statusHolder.Time);
            }
        }



        protected override void Flush()
        {
            _status = AppearStatus.Unknown;
            _statusHolder = StatusHolder.Empty;
            _userUid = "";
            _firstLineInfo.Clear();
            _secondLineInfo.Clear();
            _helpLineInfo.Clear();
            
        }

        /// <summary>
        /// Check slide average by RSSI
        /// </summary>
        /// <param name="beacon"></param>
        private void CheckSlideAverage(BeaconItem beacon)
        {
            Update(beacon.Time);


            if ((_secondLineInfo - BeaconInfoGroup.Max(_firstLineInfo, _helpLineInfo)) > rssiBarier)
                ChangeStatus(AppearStatus.Inside, beacon.Time);
            else if ((_firstLineInfo - _secondLineInfo) > rssiBarier)
                ChangeStatus(AppearStatus.Outside, beacon.Time);
        }

        private void CheckActualRssi(DateTime actualTime)
        {
            //Update(actualTime);

            report.BeginNewItem();
            report.Current.Time = actualTime.TimeOfDay.ToString();

            report.Current.FL_Aver_Rssi = 
                _firstLineInfo.ValueToCompare.Equals(double.MinValue) ? "": _firstLineInfo.ValueToCompare.ToString();

            report.Current.SL_Aver_Rssi = 
                _secondLineInfo.ValueToCompare.Equals(double.MinValue) ? "" : _secondLineInfo.ValueToCompare.ToString();

            if (_secondLineInfo > _firstLineInfo)
            {
                ChangeStatus(AppearStatus.Inside, actualTime);

                //RequestStatusChanger(new StatusHolder { Status = AppearStatus.Inside, Time = actualTime });
            }
            else if (_firstLineInfo > _secondLineInfo)
            {
                ChangeStatus(AppearStatus.Outside, actualTime);

                //RequestStatusChanger(new StatusHolder { Status = AppearStatus.Outside, Time = actualTime });
            }

            report.Current.Position = _status;
        }

        

        private void RequestStatusChanger(StatusHolder holder)
        {
            if (!_statusHolder.Equals(StatusHolder.Empty)) ///Have no status changes
            {
                if (holder.Status != _statusHolder.Status)
                {
                    _statusHolder = StatusHolder.Empty;
                }

            }
            else
            {
                _statusHolder = holder;
            }
        }
        #endregion



        private struct StatusHolder
        {
            public AppearStatus Status;
            public DateTime Time;

            internal static StatusHolder Empty => new StatusHolder { Status = AppearStatus.Unknown, Time = DateTime.MinValue };
        }
    }
}
