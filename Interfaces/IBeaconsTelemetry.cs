using System;
using System.Collections.Generic;

namespace Trigger.Interfaces
{
    public interface IBeaconsTelemetry  //интерфейс общей телеметрии
    {
        int Type { get; set; }
        ITelemetryData Data { get; set; }
    }

    public interface ITelemetryData
    {
        string UserId { get; }
        IList<IAccessPoint> APoints { get; }
    }

    public interface IAccessPoint
    {
        string Uid { get; }
        IList<ISingleBeaconValue> Beacons { get; }
    }

    public interface ISingleBeaconValue
    {
        string Mac { get; set; }
        IList<IRssiValue> Values { get; set; }
    }

    public interface IRssiValue
    {
        int Rssi { get; set; }
        DateTime Time { get; set; }
    }
}
