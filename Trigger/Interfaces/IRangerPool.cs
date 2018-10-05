using System;
using System.Collections.Generic;
using System.Text;

namespace Trigger.Interfaces
{
    public interface IRangerPool : 
        IObjectPool<string, IRanger>
        , ITriggerEvents
    { }
}
