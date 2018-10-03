using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes;
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
                                 new AccessPoint
                                 {
                                      Uid = NewGuid(),
                                      Beacons = new []
                                      {
                                          new SingleBeaconTelemetry
                                          {
                                               Mac = "00:00:00:00:00:00",
                                               Values = new [] {
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },

                                                }
                                          },
                                          new SingleBeaconTelemetry
                                          {
                                               Mac = "00:00:00:00:00:01",
                                               Values = new [] {
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },

                                                }
                                          },
                                          new SingleBeaconTelemetry
                                          {
                                               Mac = "00:00:00:00:00:02",
                                               Values = new [] {
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },

                                                }
                                          },
                                          new SingleBeaconTelemetry
                                          {
                                               Mac = "00:00:00:00:00:03",
                                               Values = new [] {
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },

                                                }
                                          },
                                          new SingleBeaconTelemetry
                                          {
                                               Mac = "00:00:00:00:00:04",
                                               Values = new [] {
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },
                                                    new RssiValue {
                                                        Rssi = - rand.Next(20, 140),
                                                        Time = DateTime.Now
                                                    },

                                                }
                                          }
                                      }
                                 }
                        }));

                return group;
            };

            yield return new[] { a() };
        }

        private static string NewGuid()
        => Guid.NewGuid().ToString();
    }
}
