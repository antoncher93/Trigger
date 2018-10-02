using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger
{
    static class RangerSettings
    {
        private static TimeSpan? beaconLifeTime = new TimeSpan?(new TimeSpan(0,0,2));
        private static int slideAverageCount = 3;

        public static int SlideAverageCount { get => slideAverageCount; set => slideAverageCount = value; }
        public static TimeSpan? BeaconLifeTime { get => beaconLifeTime; set => beaconLifeTime = value; }
    }
}
