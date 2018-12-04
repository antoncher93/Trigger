using System;
using Trigger.Beacons;
using Trigger.Classes;
using Trigger.Classes.Logging;
using Trigger.Interfaces;
using Trigger.Rangers;

namespace Trigger
{
    public class RangerBuilder : TwoLineRangerBuilder
    {
        public RangerBuilder() : base()
        {
            ranger = new Ranger();
        }

        public new RangerBuilder Modify(Action act)
        {
            return (RangerBuilder)base.Modify(act);
        }

        public RangerBuilder SetCalcSlideAverageCount(int value)
            => Modify(() => { (ranger as Ranger).slideAverageCount = value; });

        public RangerBuilder SetActualPeriod(int milliseconds)
            => Modify(() => { (ranger as Ranger)._actualSignalPeriod = milliseconds; });

        public RangerBuilder SetLogger(ILogger logger)
            => Modify(() => { (ranger as Ranger)._logger = logger; });
    }
}