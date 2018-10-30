using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Interfaces;
using Trigger.Signal;
using System.Linq;
using Trigger.Classes.Logging;

namespace Trigger.Classes
{
    public class DirectRanger// : IRanger
    {
        private BeaconInfo _beaconA;
        private BeaconInfo _beaconB;

        internal double _baseDistance = 3.0;
        internal ILogger _logger;

        private AppearStatus _status = AppearStatus.Unknown;
        private DateTime currentTime;

        public event EventHandler<TriggerEventArgs> OnEvent;

        public DirectRanger(IBeaconBody a, IBeaconBody b, int period)
        {
            _beaconA = new BeaconInfo(a.Address, period);
            _beaconB = new BeaconInfo(b.Address, period);
        }

        public DirectRanger(IBeaconBody a, IBeaconBody b, int period, ILogger logger)
        {
            _beaconA = new BeaconInfo(a.Address, period, logger);
            _beaconB = new BeaconInfo(b.Address, period, logger);
            _logger = logger;
        }

        public void CheckTelemetry(Telemetry telemetry)
        {
            var all_signals = telemetry.SelectMany(b => 
                b.Select(s => new { Mac = b.Address, Signal = s })).OrderBy(d => d.Signal.Time);

            var current = all_signals.FirstOrDefault();
            while(current != null)
            {
                var similarTime = all_signals.Where(s => s.Signal.Time == current.Signal.Time);

                foreach(var signal in similarTime)
                {
                    UpdateBeaconInfo(signal.Mac, signal.Signal);
                }
                currentTime = current.Signal.Time;

                CheckByDistance();

                current = all_signals.FirstOrDefault(s => s.Signal.Time > current.Signal.Time);
            }
        }


        private void CheckByDistance()
        {
            if(_beaconA.Distance < _beaconB.Distance)
            {
                ChangeStatus(AppearStatus.Outside);
            }
            else if(_beaconA.Distance > _beaconB.Distance)
            {
                ChangeStatus(AppearStatus.Inside);
            }
        }

        private void ChangeStatus(AppearStatus value)
        {
            if(_status != value && _status != AppearStatus.Unknown)
            {
                OnEvent?.Invoke(this, new TriggerEventArgs
                {
                    Type = value == AppearStatus.Inside ? Enums.TriggerEventType.Enter : Enums.TriggerEventType.Exit,
                    Timespan = currentTime
                });
            }

            _status = value;
        }

        public bool IsObsolete()
        {
            return false;
        }

        private void UpdateBeaconInfo(string mac, BeaconItem signal)
        {
            if(string.Equals(mac, _beaconA.MacAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                _beaconA.Add(signal);
                _beaconB.Update(signal.Time);
            }

            else if (string.Equals(mac, _beaconB.MacAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                _beaconB.Add(signal);
                _beaconA.Update(signal.Time);
            }
        }
    }
}
