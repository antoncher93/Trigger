using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Trigger.Classes.Beacons;

namespace Trigger.Beacons
{
    public class BeaconInfo
    {
        public string MacAddress { get; private set; }
        public DateTime LastSignalTime { get; private set; }
        public ActualSignals ActualSignals { get; private set; }
        public int ActualPeriod
        {
            set => ActualSignals.ActualPeriod = new TimeSpan(0, 0, 0, 0, value);
        }

        public BeaconInfo(string mac, int milliseconds)
        {
            MacAddress = mac;
            ActualSignals = new ActualSignals(milliseconds);
        }

        public void SetLastRssi(BeaconItem info)
        {
            LastSignalTime = info.Time;
            ActualSignals.Add(info);
        }
    }


}
