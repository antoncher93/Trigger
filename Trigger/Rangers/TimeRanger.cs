using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Trigger.Signal;
using Trigger.Beacons;

namespace Trigger.Rangers
{
    public class TimeRanger : TwoLineRanger
    {
        private string _report = "";

        public override string Report => _report;

        protected override void ProduceEvents(Telemetry telemetry)
        {
            base.ProduceEvents(telemetry);

            Telemetry accessible = telemetry[_firstLineBeacons.Union(_secondLineBeacons).Union(_helpBeacons)];

            if (!accessible.Any())
                return;

            _userUid = accessible.UserId;

            var data = accessible.SelectMany(beacon => beacon.Select(beaconItem => new { mac = beacon.Address, Item = beaconItem })).OrderBy(x => x.Item.Time);

            if (!data.Any())
                return;

            // refactoring ----
            IEnumerable<DateTime> checkPoints = data.Select(d => d.Item.Time).Distinct();

            foreach (DateTime date in checkPoints)
            {
                foreach (var beaconSignal in data.Where(beaconSignal => beaconSignal.Item.Time == date))
                {
                    RefreshBeaconInfoGroup(beaconSignal.mac, beaconSignal.Item);
                    Update(date);
                }
            }
            CheckRssiPeacks();

            Flush();
        }


        private void CheckRssiPeacks()
        {


            /// If some line does not have any last signal
            if (_firstLineInfo.RssiPeak.Equals(BeaconItem.Default) || _secondLineInfo.RssiPeak.Equals(BeaconItem.Default))
            {
                _report = "Some line does not have any Rssi Peak.";
                return;
            }
                


            var peak1 = _firstLineInfo.RssiPeak;
            var peak2 = _secondLineInfo.RssiPeak;
            Enums.TriggerEventType activity = Enums.TriggerEventType.Enter;
            DateTime time;

            if ((peak2 - peak1) >= TimeSpan.FromMilliseconds(0))
            {
                activity = Enums.TriggerEventType.Enter;
                time = peak2.Time;
            }
            else
            {
                activity = Enums.TriggerEventType.Enter;
                time = peak1.Time;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"Peak 1 Rssi: {peak1.Rssi} {peak1.Time.TimeOfDay}");
            sb.AppendLine();
            sb.Append($"Peak 2 Rssi: {peak2.Rssi} {peak2.Time.TimeOfDay}");
            sb.AppendLine();
            sb.Append($"Result: {activity.ToString()}");
            _report = sb.ToString();
            RaiseEvent(activity, time);
            
        }
    }
}
