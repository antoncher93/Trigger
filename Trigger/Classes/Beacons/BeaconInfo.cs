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
        

        public ActualSignals ActualSignals { get; private set; } = new ActualSignals();

        public BeaconInfo(string mac)
        {
            MacAddress = mac;
        }

        public void SetLastRssi(BeaconItem info)
        {
            LastSignalTime = info.Time;
            ActualSignals.Add(info);
        }
    }


}
