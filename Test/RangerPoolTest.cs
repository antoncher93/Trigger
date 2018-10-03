﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Trigger.Interfaces;
using Trigger.Signal;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Trigger.Test
{
    public class RangerPoolTest : BaseTest
    {
        [Fact]
        public void Test_TelemetryGroup_Binding_10x6()
        {
            // Perform
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TelemetryGroup group = DataSource.GetTelemerty_10x6().First()[0] as TelemetryGroup;
            sw.Stop();

            // Validate
            Assert.Equal(1000000, group.Count());
            Assert.True(sw.ElapsedMilliseconds < 12 * 1000);
        }

        [Theory]
        [MemberData(nameof(DataSource.GetTelemerty_10x6), MemberType = typeof(DataSource))]
        public void Test_Stress_Check(TelemetryGroup group)
        {
            string r = JsonConvert.SerializeObject(group.First().Value);
            // Prepare
            IObjectPool<string, Ranger> pool = GetNewPool();

            // Pre-validate
            Assert.NotNull(pool);

            // Perform
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Parallel.ForEach(group, (t) =>
            {
                foreach (var a in t.Value)
                    pool[a.Key].CheckTelemetry(t.Value);
            });
            sw.Stop();

            // Post-validate
            Assert.True(sw.ElapsedMilliseconds < 23 * 1000);
        }
    }
}