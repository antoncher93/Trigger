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
            string mac = "115533";
            int rssi = -79;
            
            DateTime time = DateTime.Now;

            //telemetry.NewBeacon(mac, rssi, time);

            // Validate

            Assert.True(telemetry[mac].Count == 1);

        }
        [Fact]
        public void TelemetryCutBelowTest()
        {
            // Perform
            Telemetry telemetry = Telemetry.EmptyForUser("custom");


            DateTime time_to_cut = DateTime.Now - TimeSpan.FromMinutes(5);

            telemetry.Append(
                BeaconData.FromAddress("123")
                .Add(
                    new BeaconItem[]
                    {
                        new BeaconItem { Rssi = -80, Time = DateTime.Now - TimeSpan.FromMinutes(10) },
                        new BeaconItem { Rssi = -80, Time = DateTime.Now - TimeSpan.FromMinutes(8) },
                        new BeaconItem { Rssi = -80, Time = DateTime.Now - TimeSpan.FromMinutes(3) }
                    }));

            telemetry.CutBelow(time_to_cut);

            Assert.True(telemetry.Count==1);

        }

    }
}
