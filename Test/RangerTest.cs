using System;
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

        }

        [Fact]
        public void TriggerEnterTest()
        {
            bool result = false;
            
            //RangerPool pool = new RangerPool(new Dummy2RangerSettings());

            IRanger ranger  = new RangerBuilder()
                .AddFirstLineBeacon(BeaconBody.FromMac("DF:20:C6:5A:62:5F"))
               .AddSecondLineBeacon(BeaconBody.FromMac("DE:A6:78:08:52:A2"))
               .Build();

            ranger.OnEvent += (s, e) =>
            {
                if (e.Type == Enums.TriggerEventType.Enter)
                    result = true;
            };
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
