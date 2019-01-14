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
        ConcurrentDictionary<string, IRanger>
        , IRangerPool
    {
        private readonly ITwoLineRangerBuilder _twoLineRangerBuilder;
        private readonly IRangerSettings _rangerSettings = null;

        public RangerPool(ITwoLineRangerBuilder twoLineRangerBuilder , IRangerSettings rangerSettings)
        {
            _twoLineRangerBuilder = twoLineRangerBuilder;
            _rangerSettings = rangerSettings;
        }

        public IRanger this[string key]
        {
            get
            {
                if (key == null)
                    return null;

                return GetOrAdd(key, (k) =>
                  {                      
                      //   .SetLogger(_rangerSettings.GetLogger());
                      _twoLineRangerBuilder.SetSpaceUid(key);
                      Action<BeaconLine, Action<IBeaconBody>> fillLine = (type, action) =>
                      {
                          IEnumerable<IBeaconBody> beacons = _rangerSettings.GetBeaconsBySpace(key, type);
                          if (beacons != null)
                              foreach (var b in beacons)
                                  action(b);
                      };

                      fillLine(BeaconLine.First, (b) => { _twoLineRangerBuilder.AddFirstLineBeacon(b); });
                      fillLine(BeaconLine.Second, (b) => { _twoLineRangerBuilder.AddSecondLineBeacon(b); });
                      fillLine(BeaconLine.Help, (b) => { _twoLineRangerBuilder.AddHelpBeacon(b); });

                      _twoLineRangerBuilder.SetActualSignalCount(_rangerSettings.GetActualSignalCount());
                      _twoLineRangerBuilder.SetActualSignalPeriod(_rangerSettings.GetActualSignalPeriod());

                      IRanger result = _twoLineRangerBuilder.Build();

                      result.OnEvent += OnEvent;

                      return result;
                  });
            }
        }

        public event EventHandler<TriggerEventArgs> OnEvent;

        public void Flush()
        {
            IRanger removed;
            foreach (var r in this)
                if (r.Value.IsObsolete())
                    TryRemove(r.Key, out removed);
        }
    }
}