using Newtonsoft.Json;
using System;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Signal;
using Xunit;

namespace Trigger.Test
{
    public class SerializerTest : BaseTest
    {
        [Theory]
        [MemberData(nameof(DataSource.GetTelemerty_10x6), MemberType = typeof(DataSource))]
        public void Test_Serialization_Check(TelemetryGroup group)
        {
            // Perform
            foreach (var t in group)
            {
                string serialized = JsonConvert.SerializeObject(t.Value, new TelemetryJsonConverter());
                Telemetry deserialize = JsonConvert.DeserializeObject<Telemetry>(serialized, new TelemetryJsonConverter());

                Assert.Equal(t.Value.Type, deserialize.Type);
                Assert.Equal(t.Value.Count, deserialize.Count);
            }
        }
    }
}