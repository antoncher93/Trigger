using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trigger.Signal;
using Xunit;

namespace Trigger.Test
{
    public class TelemetryFromFileTest : BaseTest
    {
        [Fact]
        public void GetTelemetryFromRes()
        {
            Telemetry res = DataSource.GetTelemetryFromResource();
            Assert.True(res != null);
        }

    }
}
