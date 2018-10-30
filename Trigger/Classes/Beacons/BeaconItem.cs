using System;

namespace Trigger.Beacons
{
    public struct BeaconItem
    {
        public int Rssi { get; set; }
        public DateTime Time { get; set; }

        public static BeaconItem Default
        {
            get
            {
                return new BeaconItem
                {
                    Rssi = 0,
                    Time = DateTime.MinValue
                };
            }
        }

        public static BeaconItem FromCompact(long value, DateTime offset)
        {
            if (value < 0)
                value = Math.Abs(value);

            var rssi = - Convert.ToInt32(value / 1000000000);
            var sec = (value - Math.Abs((long)rssi) * 1000000000) / 10.0; // shift In tenths of second

            var time = offset.AddSeconds(sec);
            return new BeaconItem { Rssi = rssi, Time = time };
        }

        public long ToCompact(DateTime offset)
        {
            if (offset > Time)
                throw new ArgumentException("Incorrect date offset");

            // Calculate time shift In tenths of second
            long delta = (long)((Time - offset).TotalMilliseconds / 100.0);

            return Math.Abs((long)Rssi) * 1000000000 + delta;
        }

        public override string ToString()
        {
            return $"{Rssi} {Time.ToString("hh:mm:ss.fff")}";
        }
    }
}
