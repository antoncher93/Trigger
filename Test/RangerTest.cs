using System;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
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

            RangerPool pool = new RangerPool(new DummyRangerSettings());

            foreach (var key in telemetry.Keys)
            {
                IRanger ranger = pool[key];
                ranger.OnEvent += (s, e) =>
                {
                    switch(e.Type)
                    {
                        case Enums.TriggerEventType.Enter:
                            result = true;
                            break;
                        case Enums.TriggerEventType.Exit:
                            result = false;
                            break;
                    }
                    count++;
                };
                ranger.CheckTelemetryByActualRssi(telemetry);
            }

            

            Assert.True(result);
        }

        [Fact]
        public void ActualSignalTest()
        {
            bool result = true;

            ActualSignals signals = new ActualSignals();

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
                .SetSignalLifePeriod(500)
                .SetAPointUid("B4B1DDB2-6941-40BE-AC8C-29F4E5043A8A")
                .Build();

            Assert.True(ranger != null);
        }
    }
}
