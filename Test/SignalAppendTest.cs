using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using Trigger.Classes.Beacons;
using Trigger.Signal;
using Xunit;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class SignalAppendTest : BaseTest
    {
        public SignalAppendTest(ITestOutputHelper helper) : base(helper)
        {

        }

        [Fact]
        public void AddSingleSignalToTelemetryTest()
        {
            // Perform
            Telemetry telemetry = Telemetry.EmptyForUser("custom");
            string uuid = "115533";
            int rssi = -79;
            
            DateTime time = DateTime.Now;


            telemetry.Append(
                  new BeaconData
                  {
                      Address = uuid
                  }.Add(new BeaconItem[]
                  {
                         new BeaconItem
                        {
                            Rssi = rssi,
                            Time = time
                        }
                  }));

            //telemetry.NewBeacon(mac, rssi, time);

            // Validate

            Assert.True(telemetry[uuid].Count == 1);

        }
    }
}
