using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Interfaces;

namespace Trigger.Rangers
{
    public abstract class BaseRangerBuilder : IRangerBuilder
    {
        protected IRanger ranger;

        protected IRangerBuilder Modify(Action act)
        {
            act?.Invoke();
            return this;
        }

        public IRanger Build() => ranger;
    }
}
