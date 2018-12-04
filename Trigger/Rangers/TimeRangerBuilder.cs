using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Rangers
{
    public class TimeRangerBuilder : TwoLineRangerBuilder
    {
        public TimeRangerBuilder() : base()
        {
            ranger = new TimeRanger();
        }
    }
}
