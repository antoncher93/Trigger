using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Signal;
using Trigger.Classes;

namespace Trigger.Rangers
{
    public abstract class TwoLineRanger : BaseRanger
    {
        internal List<IBeaconBody> _firstLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _secondLineBeacons { get; private set; } = new List<IBeaconBody>();
        internal List<IBeaconBody> _helpBeacons { get; private set; } = new List<IBeaconBody>();

        protected BeaconInfoGroup _firstLineInfo = new BeaconInfoGroup();
        protected BeaconInfoGroup _secondLineInfo = new BeaconInfoGroup();
        protected BeaconInfoGroup _helpLineInfo = new BeaconInfoGroup();

        internal int _actualSignalPeriod = 1000;

        public override void OnNext(Telemetry value)
        {
            ProduceEvents(value);
        }

        protected void RefreshBeaconInfoGroup(MacAddress macAddress, BeaconItem beacon)
        {
            bool flag = false;

            Action<IList<IBeaconBody>, BeaconInfoGroup> CheckBeacon = (line, group) =>
            {
                if (flag)
                    return;

                if (line.Any(b => b.Address == macAddress))
                {
                    flag = true;

                    var res = group.FirstOrDefault(b => b.MacAddress == macAddress);
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

        protected virtual void ProduceEvents(Telemetry telemetry)
        {
            
        }

        protected virtual void Update(DateTime actual_time) // проверить всемя последних сигналов от линий
        {
            _firstLineInfo.Update(actual_time);
            _secondLineInfo.Update(actual_time);
            _helpLineInfo.Update(actual_time);
        }

        protected virtual void Flush()
        {
            _userUid = "";
            _firstLineInfo.Clear();
            _secondLineInfo.Clear();
            _helpLineInfo.Clear();
        }
    }
}
