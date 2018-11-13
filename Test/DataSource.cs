using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Signal;

namespace Trigger.Test
{
    public class DataSource
    {

        public static IEnumerable<object[]> GetTelemerty_10x6()
        {
            Random rand = new Random(1);

            Func<TelemetryGroup> a = () =>
            {
                IList<Telemetry> items = new List<Telemetry>();

                for (int i = 0; i < 1000000; i++)
                {
                    var telemetry = Telemetry.EmptyForUser(NewGuid())
                    .Append(
                        new BeaconData
                        {
                            Address = "c9:18:b1:cf:9b:50"
                        }.Add(
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            }
                            )
                         )
                         .Append(
                        new BeaconData
                        {
                            Address = "de:a6:78:08:52:a2",
                        }.Add(
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            },
                            new BeaconItem
                            {
                                Rssi = -rand.Next(20, 140),
                                Time = RandDateTime(rand)
                            }));

                    items.Add(telemetry);
                }

                TelemetryGroup group = new TelemetryGroup(items);

                return group;
            };

            yield return new[] { a() };
        }

        private static DateTime RandDateTime(Random rand)
        {
            return DateTime.Now.AddSeconds(rand.Next(-1000, 1000))
                .AddMilliseconds(rand.Next(-1000, 1000));
        }

        private static string NewGuid()
        => Guid.NewGuid().ToString();

        public static Telemetry GetTelemetryFromResource()
        {
            Telemetry telemetry = null;
            var assembly = Assembly.GetExecutingAssembly();
            using (var telemetry_stream = assembly.GetManifestResourceStream("Trigger.Test.Resources.telemetry.txt"))
            {
                StreamReader reader = new StreamReader(telemetry_stream);
                string str = reader.ReadToEnd();
                telemetry = Newtonsoft.Json.JsonConvert.DeserializeObject<Telemetry>(str, new TelemetryJsonConverter());
            }

            return telemetry;
        }


        
    }


}
