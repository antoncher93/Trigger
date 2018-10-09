using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Trigger.Beacons;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Trigger.Classes
{
    /// <summary>
    /// Dictionary key is access point uid
    /// </summary>
    public class RangerPool :
        ConcurrentDictionary<string, Ranger>
        , IRangerPool
    {
        private readonly IRangerSettings _rangerSettings = null;

        public RangerPool(IRangerSettings rangerSettings)
        {
            _rangerSettings = rangerSettings;
        }

        public IRanger this[string key]
        {
            get
            {
                return GetOrAdd(key, (k) =>
                  {
                      RangerBuilder builder = new RangerBuilder()
                          .SetAPointUid(key)
                          .SetCalcSlideAverageCount(_rangerSettings.GetSlideAverageCount(key));

                      Action<BeaconLine, Action<IBeaconBody>> fillLine = (type, action) =>
                      {
                          IEnumerable<IBeaconBody> beacons = _rangerSettings.GetBeacons(key, type);
                          if (beacons != null)
                              foreach (var b in beacons)
                                  action(b);
                      };

                      fillLine(BeaconLine.First, (b) => { builder.AddFirstLineBeacon(b); });
                      fillLine(BeaconLine.Second, (b) => { builder.AddSecondLineBeacon(b); });
                      fillLine(BeaconLine.Help, (b) => { builder.AddHelpBeacon(b); });

                      Ranger result = builder.Build();

                      result.OnEvent += OnEvent;

                      return result;
                  });
            }
        }

        public event EventHandler<TriggerEventArgs> OnEvent;

        public void Flush()
        {
            Ranger removed;
            foreach (var r in this)
                if (r.Value.IsObsolete())
                    TryRemove(r.Key, out removed);
        }
    }
}