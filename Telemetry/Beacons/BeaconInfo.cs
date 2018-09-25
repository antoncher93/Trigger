using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Telemetry.Beacons
{
    public class BeaconInfo
    {
        public string MacAddress { get; private set; }
        public double LastRssi { get; private set; }
        public double AverageRssi { get; private set; }
        public double MaxRssi { get; private set; }
        public double SlideAverageRssi { get; private set; }
        public DateTime LastRssiTime { get; private set; }
        public RssiPeak Peak { get; private set; }

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
            for(int i = slideCollect.Count - 1; i>=0; i--)
            {
                slideCollect.RemoveAt(i);
            }

            slideCollect = new List<int>();
        }

        public void SetLastRssi(int value, DateTime time)
        {
            count++;
           
            LastRssi = value;
            LastRssiTime = time;

            if (value > MaxRssi) MaxRssi = value;

            AverageRssi = ((AverageRssi * (count - 1)) + value) / count;

            if (slideCollect.Count >= SlideAverageCount)
            {
                slideCollect.RemoveAt(0);
            }

            slideCollect.Add(value);
            SlideAverageRssi = CalcSlideAverageRssi();

            if (slideCollect.Count > 1 && SlideAverageRssi < slideCollect[slideCollect.Count - 1])
            {
                if (Peak == null || Peak.Rssi < SlideAverageRssi)
                {
                    //if(MacAddress == "E3:25:3E:0A:7E:4C")
                    //{

                    //}
                    Peak = new RssiPeak { Rssi = SlideAverageRssi, Time = time };
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
