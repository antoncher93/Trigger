using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trigger.Telemetry.Beacons
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

        public void SetRssiValue(Beacon beacon)
        {
            var foundbeacon = beacons.FirstOrDefault(b => string.Equals(b.MacAddress, beacon.Mac, StringComparison.CurrentCultureIgnoreCase));
            if (foundbeacon == null)
            {
                foundbeacon = new BeaconInfo(beacon.Mac);
                this.Add(foundbeacon);
            }

            foundbeacon.SetLastRssi(beacon.Rssi, beacon.DateTime);
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
        #endregion
    }
}
