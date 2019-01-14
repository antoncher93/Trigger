using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Xunit;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class BeaconInfoGropTest : BaseTest
    {
        public BeaconInfoGropTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void SignalFilter_Test()
        {
            //arrange
            BeaconInfo info = new BeaconInfo("a", 1000);
            var time = DateTime.Now;
            DateTime t1 = time - TimeSpan.FromMilliseconds(1500);
            DateTime t2 = time - TimeSpan.FromMilliseconds(1400);
            DateTime t3 = time - TimeSpan.FromMilliseconds(1300);

            BeaconItem item1 = new BeaconItem { Rssi = -60, Time = t1 };
            BeaconItem item2 = new BeaconItem { Rssi = -61, Time = t2 };
            BeaconItem item3 = new BeaconItem { Rssi = -62, Time = t3 };

            info.Add(item1);
            info.Add(item2);
            info.Add(item3);

            Assert.Equal(info.ValueToCompare, -61);

            DateTime t4 = time - TimeSpan.FromMilliseconds(1200);
            BeaconItem item4 = new BeaconItem { Rssi = -55, Time = t3 };
            info.Add(item4);

            Assert.Equal(info.ValueToCompare, -55);


        }
    }
}
