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

        private int count;
        private List<int> slideCollect;
        private int slideCount = 5;
        public int SlideAverageCount
        {
            get { return slideCount; }
            set { slideCount = value; }
        }


        public BeaconInfo(string mac)
        {
            MacAddress = mac;
            LastRssi = 0;
            AverageRssi = 0;
            MaxRssi = -100;
            SlideAverageRssi = -100;
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

            if(MacAddress == "DE:A6:78:08:52:A2")
            {

            }

            LastRssi = value;
            LastRssiTime = time;

            if (value > MaxRssi) MaxRssi = value;

            AverageRssi = ((AverageRssi * (count - 1)) + value) / count;

            if (slideCollect.Count >= slideCount)
            {
                slideCollect.RemoveAt(0);
            }

            slideCollect.Add(value);
            SlideAverageRssi = CalcSlideAverageRssi();
          
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
    }
}
