using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Interfaces;

namespace Trigger.Rangers
{
    public abstract class BaseRangerBuilder<T> : IRangerBuilder<T> where T : IRanger
    {
        protected T ranger;

        protected IRangerBuilder<T> Modify(Action act)
        {            
            act?.Invoke();
            return this;
        }

        public T Build() => ranger;
    }
}
