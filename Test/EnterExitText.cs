using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes;
using Trigger.Signal;
using Xunit;

namespace Trigger.Test
{
    
    public class EnterExitText : BaseTest
    {
        [Fact]
        public void TriggerEnterTest()
        {
            bool result = false;
            int count = 0;
            Telemetry telemetry = DataSource.GetTelemetryFromResource();

            RangerPool pool = new RangerPool(new DummyRangerSettings());

            foreach(var key in telemetry.Keys)
            {
                Ranger ranger = pool[key];

                ranger.OnEnter += (s, e) =>
                {
                    result = true;
                    count++;
                };

                ranger.OnExit += (s, e) =>
                {
                    result = false;
                    count++;
                };

                ranger.CheckTelemetry(telemetry);
            }

            Assert.True(result && count <= 3);
        }
    }
}
