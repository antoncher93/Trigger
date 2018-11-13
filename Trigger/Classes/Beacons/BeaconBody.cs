using System;
using Trigger.Classes;

namespace Trigger.Beacons
{
    public class BeaconBody : IBeaconBody
    {
        public string Address { get; protected set; }

        public static BeaconBody FromMac(MacAddress address)
        {
            return new BeaconBody { Address = address };
        }

        public static BeaconBody FromUUID(Guid uuid)
        {
            return new BeaconBody { Address = uuid.ToString().Trim() };
        }

        public override string ToString()
        {
            return Address;
        }
    }
}