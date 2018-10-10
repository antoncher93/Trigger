using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trigger.Beacons
{
    public class BeaconInfoGroup : ICollection<BeaconInfo>
    {
        #region//from ICollection

        private IList<BeaconInfo> beacons;

        public int Count => beacons.Count;

        public bool IsReadOnly => beacons.IsReadOnly;

        public void Add(BeaconInfo item)
        {
            item.SlideAverageCount = this.SlideAverageCount;
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

        public int SlideAverageCount
        {
            get; set;
        }
        public BeaconInfoGroup()
        {
            beacons = new List<BeaconInfo>();
            SlideAverageCount = 3;
        }

        private TimeSpan _timeOffset = new TimeSpan(0, 0, 2);
        //public bool Changed { get; private set; } = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="macAddress">Beacon mac</param>
        /// <param name="beacon"></param>
        public void SetRssiValue(string macAddress, BeaconItem beacon)
        {
            var foundbeacon = beacons.FirstOrDefault(b => string.Equals(b.MacAddress, macAddress, StringComparison.CurrentCultureIgnoreCase));
            if (foundbeacon == null)
            {
                foundbeacon = new BeaconInfo(macAddress);
                this.Add(foundbeacon);
            }

            foundbeacon.SetLastRssi(beacon);
        }

        public double MaxSlideRssi
        {
            get
            {
                if (beacons.Count > 0)
                {
                    return beacons.OrderByDescending(b => b.SlideAverageRssi).FirstOrDefault().SlideAverageRssi;
                }
                else return -200;
            }
        }

        public double MaxLastRssi
        {
            get
            {
                return beacons.OrderByDescending(b => b.LastRssi).FirstOrDefault().LastRssi;
            }
        }

        #region Operations
        public static double operator -(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if(a == null || b== null)
            {
                throw new NullReferenceException();
            }
            return a.MaxSlideRssi - b.MaxSlideRssi;
        }

        public static bool operator <(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return true;

            if (a != null && b == null)
                return false;

            return a.MaxSlideRssi < b.MaxSlideRssi;
        }

        public static bool operator >(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return false;

            if (a != null && b == null)
                return true;

            return a.MaxSlideRssi > b.MaxSlideRssi;
        }

        public static bool operator <=(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return true;

            if (a != null && b == null)
                return false;

            return a.MaxSlideRssi <= b.MaxSlideRssi;
        }

        public static bool operator >=(BeaconInfoGroup a, BeaconInfoGroup b)
        {
            if (a == null && b == null)
                return false;

            if (a == null && b != null)
                return false;

            if (a != null && b == null)
                return true;

            return a.MaxSlideRssi >= b.MaxSlideRssi;
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

        public void UpdateSlideAverageRssi(DateTime actual_time)
        {
            foreach(var beacon in beacons)
            {
                if(actual_time - beacon.LastRssiTime > new TimeSpan?(_timeOffset) )
                {
                    beacon.ResetSlideAverageRssi();
                }
            }
        }
        #endregion
    }
}
