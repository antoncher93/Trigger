using Trigger.Signal;

namespace Trigger.Interfaces
{
    public interface IRanger
    {
        void CheckTelemetry(Telemetry telemetry);

        bool IsObsolete();
    }
}
