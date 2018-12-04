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
            if (_firstLineInfo.LastSignal.Equals(BeaconItem.Default) || _secondLineInfo.LastSignal.Equals(BeaconItem.Default))
                return;

            if ((_secondLineInfo.RssiPeak - _firstLineInfo.RssiPeak) >= TimeSpan.FromMilliseconds(500))
            {
                RaiseEvent(Enums.TriggerEventType.Enter, _secondLineInfo.RssiPeak.Time);
            }
            else if (_firstLineInfo > _secondLineInfo)
            {
                RaiseEvent(Enums.TriggerEventType.Exit, _secondLineInfo.RssiPeak.Time);
            }
        }
    }
}
