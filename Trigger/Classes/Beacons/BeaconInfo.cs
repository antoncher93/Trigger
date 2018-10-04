using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Trigger.Beacons
{
    public class BeaconInfo
    {
        public string MacAddress { get; private set; }
        public int LastRssi { get; private set; }
        public double AverageRssi { get; private set; }
        public double MaxRssi { get; private set; }
        public double SlideAverageRssi { get; private set; }
        public DateTime LastRssiTime { get; private set; }
        public RssiPeak Peak { get; private set; }


        private Timer timer;

        private int count;
        private List<int> slideCollect;
        public int SlideAverageCount { get; set; }

        public BeaconInfo(string mac)
        {
            MacAddress = mac;
            LastRssi = -999;
            AverageRssi = -999;
            MaxRssi = -999;
            SlideAverageRssi = -999;
            count = 0;

            slideCollect = new List<int>();
        }

        public void ResetSlideAverageRssi()
        {
            SlideAverageRssi = -200;
            slideCollect.Clear();
        }

        public void SetLastRssi(BeaconItem info)
        {
            count++;
           
            LastRssi = info.Rssi;
            LastRssiTime = info.Time;

            if (LastRssi > MaxRssi) MaxRssi = LastRssi;

            AverageRssi = ((AverageRssi * (count - 1)) + LastRssi) / count;

            if (slideCollect.Count >= SlideAverageCount)
            {
                slideCollect.RemoveAt(0);
            }

            slideCollect.Add(LastRssi);
            SlideAverageRssi = CalcSlideAverageRssi();

            if (slideCollect.Count > 1 && SlideAverageRssi < slideCollect[slideCollect.Count - 1])
            {
                if (Peak == null || Peak.Rssi < SlideAverageRssi)
                {
                    Peak = new RssiPeak { Rssi = SlideAverageRssi, Time = LastRssiTime };
                }
            }
           
        }

        private double CalcSlideAverageRssi()
        {
            double sum = 0;
            foreach (var r in slideCollect)
            {
                sum += r;
            }

            return sum / slideCollect.Count;
        }

        public class RssiPeak
        {
            public double Rssi { get; set; }
            public DateTime Time { get; set; }
        }
    }
}
