using System;
using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface ITriggerEvents
    {
        event EventHandler<TriggerEventArgs> OnEvent;
    }
}