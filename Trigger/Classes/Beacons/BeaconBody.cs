using Trigger.Classes;

namespace Trigger.Beacons
{
    public class BeaconBody : IBeaconBody
    {
        public MacAddress Address { get; protected set; }

        public static BeaconBody FromMac(MacAddress address)
        {
            return new BeaconBody { Address = address };
        }

        public override string ToString()
        {
            return Address;
        }
    }
}