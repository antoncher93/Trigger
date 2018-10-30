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
                TelemetryGroup group = new TelemetryGroup();
                for (int i = 0; i < 1000000; i++)
                    group.Add(new Telemetry()
                    {
                        Type = Enums.TelemetryType.FromUser,
                        UserId = NewGuid()
                    }.AddRange(new[]
                                 {
                                 new Beacon
                                          {
                                               Mac = "c9:18:b1:cf:9b:50",
                                          }.Add(
                                              new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    }),
                                          new Beacon
                                          {
                                               Mac = "e3:25:3e:0a:7e:4c",

                                          }.Add(
                                              new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    }),
                                          new Beacon
                                          {
                                               Mac = "de:a6:78:08:52:a2",
                                          }.Add(
                                              new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    }),
                                          new Beacon
                                          {
                                               Mac = "c1:11:11:1b:11:1a"

                                          }.Add(
                                              new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    }),
                                          new Beacon
                                          {
                                               Mac = "fd:3a:73:b7:54:ba"

                                          }
                                          .Add(
                                              new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    },
                                                    new BeaconItem {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = RandDateTime(rand)
                                                    })
                        }));

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
