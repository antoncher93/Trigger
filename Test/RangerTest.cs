using System;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Classes.Logging;
using Trigger.Interfaces;
using Trigger.Signal;
using Xunit;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class RangerTest : BaseTest
    {
        public RangerTest(ITestOutputHelper helper)
            : base(helper)
        {

        }

        [Fact]
        public void Test_Ranger()
        {

        }

        [Fact]
        public void TriggerEnterTest()
        {
            bool result = false;
            int count = 0;
            Telemetry telemetry = DataSource.GetTelemetryFromResource();
            
            RangerPool pool = new RangerPool(new Dummy2RangerSettings());

            //foreach (var key in telemetry.Keys)
            //{
            //    IRanger ranger = pool[key];
            //    ranger.OnEvent += (s, e) =>
            //    {
            //        switch(e.Type)
            //        {
            //            case Enums.TriggerEventType.Enter:
            //                result = true;
            //                break;
            //            case Enums.TriggerEventType.Exit:
            //                result = false;
            //                break;
            //        }
            //        count++;
            //    };
            //    ranger.CheckTelemetry(telemetry);
            //}

            Assert.True(result);
        }

        [Fact]
        public void DirectTriggerTest()
        {
            bool result = false;
            int count = 0;
            //IRanger ranger = new DirectRanger(
            //    BeaconBody.FromMac("de:a6:78:08:52:a2"), 
            //    BeaconBody.FromMac("c9:18:b1:cf:9b:50"), 
            //    500, 
            //    new DBLogger());

            //ranger.OnEvent += (s, e) =>
            //{
            //    switch (e.Type)
            //    {
            //        case Enums.TriggerEventType.Enter:
            //            result = true;
            //            break;
            //        case Enums.TriggerEventType.Exit:
            //            result = false;
            //            break;
            //    }
            //    count++;
            //};
            //Telemetry telemetry = DataSource.GetTelemetryFromResource();

            //ranger.CheckTelemetry(telemetry);

            Assert.True(result);
        }

        [Fact]
        public void ActualSignalTest()
        {
            bool result = true;

            BeaconInfo signals = new BeaconInfo("MyMac", 2000);

            signals.Add(new BeaconItem { Time = DateTime.Now - new TimeSpan(0,0, 5), Rssi = -79 });
            signals.Add(new BeaconItem { Time = DateTime.Now - new TimeSpan(0, 0, 5), Rssi = -79 });

            signals.Update(DateTime.Now);

            Assert.True(signals.Count == 0);


        }

        [Fact]
        public void RangerMethodsTest()
        {
            var ranger = new RangerBuilder()
                .AddFirstLineBeacon(BeaconBody.FromMac("DF:20:C6:5A:62:5F"))
                .AddSecondLineBeacon(BeaconBody.FromMac("DE:A6:78:08:52:A2"))
                .SetCalcSlideAverageCount(3)
                .SetAPointUid("B4B1DDB2-6941-40BE-AC8C-29F4E5043A8A")
                .Build();

            Assert.True(ranger != null);
        }
    }
}
