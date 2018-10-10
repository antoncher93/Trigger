using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trigger.Signal;
using Xunit;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class TelemetryFromFileTest : BaseTest
    {
        public TelemetryFromFileTest(ITestOutputHelper helper)
            : base(helper) { }

        [Fact]
        public void GetTelemetryFromRes()
        {
            Telemetry res = DataSource.GetTelemetryFromResource();
            Assert.True(res != null);
        }

    }
}
