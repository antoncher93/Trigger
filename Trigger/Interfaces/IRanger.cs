using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface IRanger : ITriggerEvents
    {
        void CheckTelemetry(Telemetry telemetry);

        bool IsObsolete();
    }
}
