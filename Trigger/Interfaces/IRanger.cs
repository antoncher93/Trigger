using System;
using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface IRanger : ITriggerEvents, IObserver<Telemetry>
    {
        //void ProduceEvent(Telemetry telemetry);

        bool IsObsolete();

        void Subscribe(IObservable<Telemetry> provider);

        string Report { get; }
    }
}
