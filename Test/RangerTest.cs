using System;
using System.IO;
using System.Threading;

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
            bool result = false;

            var telemetry = Telemetry.EmptyForUser("test");
            telemetry
                .Append(BeaconData.FromAddress("1").Add
                (new BeaconItem[]
                {
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(10) },
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(9.5) },
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(8.5) }
                }))
                .Append(BeaconData.FromAddress("2").Add
                (new BeaconItem[]
                {
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(8) },
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(7.5) },
                    new BeaconItem {Rssi = -70, Time = DateTime.Now - TimeSpan.FromSeconds(7) }
                }));

            var ranger = new RangerBuilder()
                .AddFirstLineBeacon(BeaconBody.FromMac("1"))
                .AddSecondLineBeacon(BeaconBody.FromMac("2"))
                .Build();

            ranger.OnEvent += (s, e) => result = e.Type == Enums.TriggerEventType.Enter ? true : false;

            ranger.OnNext(telemetry);

            Assert.True(result);

        }

        [Fact]
        public void TriggerEnterTest()
        {
            bool result = false;

            string str = File.ReadAllText("D:\\Signals\\telemetry.txt");

            var telemetry = Newtonsoft.Json.JsonConvert.DeserializeObject<Telemetry>(str, new TelemetryJsonConverter());

            IRanger ranger  = new RangerBuilder()
                .AddFirstLineBeacon(BeaconBody.FromUUID(new Guid("ebefd083-70a2-47c8-9837-e7b5634df599")))
               .AddSecondLineBeacon(BeaconBody.FromUUID(new Guid("ebefd083-70a2-47c8-9837-e7b5634df525")))
               .Build();

            ranger.OnEvent += (s, e) =>
            {
                if (e.Type == Enums.TriggerEventType.Enter)
                    result = true;
            };

            ranger.OnNext(telemetry);
            Assert.True(result);
        }

        [Fact]
        public void DirectTriggerTest()
        {
            bool result = false;

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
                //.SetAPointUid("B4B1DDB2-6941-40BE-AC8C-29F4E5043A8A")
                .Build();

            Assert.True(ranger != null);
        }
    }
}
