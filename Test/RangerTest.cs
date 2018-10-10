using System;
using Trigger.Classes;
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
                ranger.CheckTelemetry(telemetry);
            }

            Assert.True(result && count <= 3);
        }
    }
}
