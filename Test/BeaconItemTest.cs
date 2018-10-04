using System;
using Trigger.Beacons;
using Xunit;

namespace Trigger.Test
{
    public class BeaconItemTest
    {
        [Fact]
        public void Test_BeaconItem_to_compact()
        {
            // Prepare
            Random rand = new Random();
            int rssi = -rand.Next(0, 1000);
            DateTime offset = DateTime.Now;
            BeaconItem item = new BeaconItem { Rssi = rssi, Time = offset
                .AddSeconds(rand.Next(0, 1000))
                .AddMilliseconds(rand.Next(0, 1000))
            };

            // Pre-validate
            Assert.True(rssi <= 0 && rssi >= -1000);

            // Perform
            long value = item.ToCompact(offset);
            BeaconItem restored = BeaconItem.FromCompact(value, offset);

            // Post-validate
            Assert.True((item.Time - restored.Time).TotalMilliseconds <= 100);
        }
    }
}
