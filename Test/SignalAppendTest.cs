using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Signal;
using Xunit;

namespace Trigger.Test
{
    public class SignalAppendTest : BaseTest
    {
        [Fact]
        public void AddSingleSignalToTelemetryTest()
        {
            // Perform
            Telemetry telemetry = Telemetry.EmptyForUser("custom");
            string mac = "115533";
            int rssi = -79;
            string apointUid = "apoint";
            DateTime time = DateTime.Now;

            telemetry.NewBeacon(mac, rssi, apointUid, time);

            // Validate

            Assert.True(telemetry[apointUid].Beacons.Count == 1);

        }
    }
}
