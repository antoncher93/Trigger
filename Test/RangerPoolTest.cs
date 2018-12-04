using System.Diagnostics;
using Trigger.Interfaces;
using Trigger.Signal;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;

namespace Trigger.Test
{
    public class RangerPoolTest : BaseTest
    {
        public RangerPoolTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        { }

        [Fact]
        public void Test_TelemetryGroup_Binding_10x6()
        {
            // Perform
            Stopwatch sw = new Stopwatch();
            sw.Start();
            TelemetryGroup group = DataSource.GetTelemerty_10x6().First()[0] as TelemetryGroup;
            sw.Stop();

            // Validate
            Assert.Equal(100000, group.Count());
            Assert.True(sw.ElapsedMilliseconds < 12 * 1000);
        }

        [Theory]
        [MemberData(nameof(DataSource.GetTelemerty_10x6), MemberType = typeof(DataSource))]
        public void Test_Stress_Check(TelemetryGroup group)
        {
            // Prepare
            IRangerPool pool = GetNewPool();

            // Pre-validate
            Assert.NotNull(pool);

            // Perform
            Stopwatch sw = new Stopwatch();

            sw.Start();
            Parallel.ForEach(group, (t) =>
            {
                //foreach (var a in t.Value)
                //    pool[a.Key].CheckTelemetry(t.Value);
            });
            sw.Stop();

            // Post-validate
            Assert.True(sw.ElapsedMilliseconds < 23 * 1000);
        }

        [Fact]
        public void Test_Pool_Event()
        {
            // Prepare
            IRangerPool pool = GetNewPool();
            bool ok = false;

            // Pre-validate
            pool.OnEvent += (sender, e) => {
                ok = true;
                _logger.LogInformation($"User '{e.UserId}' status changed at {e.Timespan}: {e.Type}");
            };

            // Perform
            //((Ranger)pool["test"]).ChangeStatus(AppearStatus.Outside);
            //((Ranger)pool["test"]).ChangeStatus(AppearStatus.Inside);

            // Post-validate
            Assert.True(ok);
        }
    }
}
