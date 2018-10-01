using System;
using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface IRangerEvents
    {
        event EventHandler<TriggerEventArgs> OnEnter;
        event EventHandler<TriggerEventArgs> OnExit;
    }
}