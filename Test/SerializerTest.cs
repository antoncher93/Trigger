using System;
using Newtonsoft.Json;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Signal;
using Xunit;
using Xunit.Abstractions;

namespace Trigger.Test
{
    public class SerializerTest : BaseTest
    {
        public SerializerTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {

        }

        [Theory]
        [MemberData(nameof(DataSource.GetTelemerty_10x6), MemberType = typeof(DataSource))]
        public void Test_Serialization_Check(TelemetryGroup group)
        {
            // Perform
            foreach (var t in group)
            {
                string serialized = JsonConvert.SerializeObject(t, new TelemetryJsonConverter());
                Telemetry deserialize = JsonConvert.DeserializeObject<Telemetry>(serialized, new TelemetryJsonConverter());

                Assert.Equal(t.Type, deserialize.Type);
                Assert.Equal(t.Count, deserialize.Count);
            }
        }

        [Fact]
        public void TestSeDesUserId()
        {
            bool result = false;
            var user_id = Guid.NewGuid().ToString();
            var telemetry = Telemetry.EmptyForUser(user_id);

            telemetry.Append(BeaconData.FromAddress("Aa")
                .Add(new BeaconItem[]
                {
                    new BeaconItem{Rssi = -80, Time = DateTime.Now }
                }));

            string s = JsonConvert.SerializeObject(telemetry, new TelemetryJsonConverter());

            var new_telemetry = JsonConvert.DeserializeObject<Telemetry>(s, new TelemetryJsonConverter());

            Assert.True(telemetry.UserId.Equals(new_telemetry.UserId, StringComparison.CurrentCultureIgnoreCase));
        }

        [Fact]
        public void TestSeDesBeacons()
        {
            var user_id = Guid.NewGuid().ToString();
            var telemetry = Telemetry.EmptyForUser(user_id);

            telemetry.Append(BeaconData.FromAddress("Aa")
                .Add(new BeaconItem[]
                {
                    new BeaconItem{Rssi = -80, Time = DateTime.Now }
                }));

            string s = JsonConvert.SerializeObject(telemetry, new TelemetryJsonConverter());

            var new_telemetry = JsonConvert.DeserializeObject<Telemetry>(s, new TelemetryJsonConverter());

            int count1 = 0, count2 = 0;

            foreach(var b in telemetry)
            {
                count1 += b.Count;
            }
            foreach (var b in new_telemetry)
            {
                count2 += b.Count;
            }


            Assert.True(count1 == count2);
        }
    }
}