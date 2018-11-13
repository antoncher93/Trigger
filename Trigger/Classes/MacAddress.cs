using System;
using System.Collections.Generic;

namespace Trigger.Classes
{
    public class MacAddress 
    {
        private string _macAddress { get; set; }

        public static implicit operator string(MacAddress value)
        {
            return value._macAddress;
        }

        public static implicit operator MacAddress(string value)
        {
            return new MacAddress { _macAddress = value.Trim().ToUpper() };
        }

        public static bool operator ==(MacAddress a, MacAddress b)
            => string.Equals(a._macAddress, b._macAddress, StringComparison.InvariantCultureIgnoreCase);

        public static bool operator !=(MacAddress a, MacAddress b)
            => !string.Equals(a._macAddress, b._macAddress, StringComparison.InvariantCultureIgnoreCase);

        public override bool Equals(object obj)
        {
            if (!(obj is MacAddress) && !(obj is string))
                return false;

            return string.Equals(_macAddress, (MacAddress)obj, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return -1356315129 + EqualityComparer<string>.Default.GetHashCode(_macAddress);
        }

        public override string ToString()
        {
            return _macAddress.Trim().ToUpper();
        }
    }
}
