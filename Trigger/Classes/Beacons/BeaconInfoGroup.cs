using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Trigger.Classes.Logging;
using Trigger.Classes;

namespace Trigger.Beacons
{
    public class BeaconInfoGroup : ICollection<BeaconInfo>
    {
        #region From ICollection

        private IList<BeaconInfo> beacons = new List<BeaconInfo>();

        public int Count => beacons.Count;

        public bool IsReadOnly => beacons.IsReadOnly;

        public void Add(BeaconInfo item)
        {
            beacons.Add(item);
        }

        public void Clear()
        {
            beacons.Clear();
        }

        public bool Contains(BeaconInfo item)
        {
            return beacons.Contains(item);
        }

        public void CopyTo(BeaconInfo[] array, int arrayIndex)
        {
            beacons.CopyTo(array, arrayIndex);
        }

        public IEnumerator<BeaconInfo> GetEnumerator()
        {
            return beacons.GetEnumerator();
        }

        public bool Remove(BeaconInfo item)
        {
            return beacons.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return beacons.GetEnumerator();
        }

        #endregion

        private ILogger _logger;
        public int ActualPeriod { get; set; } = 1000;

        public BeaconInfoGroup(ILogger logger = null)
        {
            _logger = logger;
        }

        //public bool Changed { get; private set; } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="macAddress">Beacon mac</param>
        /// <param name="beacon"></param>
        public void SetRssiValue(MacAddress macAddress, BeaconItem beacon)
        {
            var foundbeacon = beacons.FirstOrDefault(b => b.MacAddress == macAddress);

            if (foundbeacon == null)
            {
                foundbeacon = new BeaconInfo(macAddress, ActualPeriod);
                this.Add(foundbeacon);
            }

            foundbeacon.Add(beacon);
        }

        public double ValueToCompare => MaxAverRssi;

        public double MaxAverRssi
        {
            get
            {
                if (beacons.Count > 0)
                {
                    return beacons.OrderByDescending(b => b.AverageRssi).FirstOrDefault().AverageRssi;
                }
                else return -200;
            }
        }

        public void Update(DateTime actual_time)
        {
            foreach (var beacon in beacons)
            {
                beacon.Update(actual_time);
            }
        }

        #region Operations
        public static double operator -(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if(a == null || b== null)
            {
                throw new NullReferenceException();
            }
            return a.ValueToCompare - b.ValueToCompare;
        }

        public static bool operator <(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return true;

            if (a != null && b == null)
                return false;

            return a.ValueToCompare < b.ValueToCompare;
        }

        public static bool operator >(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return false;

            if (a != null && b == null)
                return true;

            return a.ValueToCompare > b.ValueToCompare;
        }

      

        public static bool operator <=(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return true;

            if (a != null && b == null)
                return false;

            return a.ValueToCompare <= b.ValueToCompare;
        }

        public static bool operator >=(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return false;

            if (a != null && b == null)
                return true;

            return a.ValueToCompare >= b.ValueToCompare;
        }

        public static BeaconInfoGroup Max(params BeaconInfoGroup[] items)
        {
            if (items.Length == 0)
                return null;

            BeaconInfoGroup max = items[0];

            foreach (var i in items)
                if (i > max)
                    max = i;

            return max;
        }

        
        #endregion
    }
}
