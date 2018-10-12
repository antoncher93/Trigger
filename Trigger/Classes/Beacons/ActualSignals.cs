using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Beacons;
using System.Linq;
using System.Collections;

namespace Trigger.Classes.Beacons
{
    public class ActualSignals : ICollection<BeaconItem>
    {
        public ActualSignals(int milliseconds)
        {
            ActualPeriod = new TimeSpan(0, 0, 0, 0, milliseconds);
            signals = new List<BeaconItem>();
        }

        private IList<BeaconItem> signals;
        public DateTime UpdateTime { get; private set; }
        public TimeSpan ActualPeriod { get; set; }

        public int Count => signals.Count;
        public bool IsReadOnly => signals.IsReadOnly;
        public void Add(BeaconItem item)
        {
            signals.Add(item);
            Update(item.Time);
        }

        public void Clear()
        {
            signals.Clear();
        }
        public bool Contains(BeaconItem item)
        {
            return signals.Contains(item);
        }
        public void CopyTo(BeaconItem[] array, int arrayIndex)
        {
            signals.CopyTo(array, arrayIndex);
        }
        public IEnumerator<BeaconItem> GetEnumerator()
        {
            return signals.GetEnumerator();
        }
        public bool Remove(BeaconItem item)
        {
            return signals.Remove(item);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return signals.GetEnumerator();
        }
        public ActualSignals Update(DateTime actualTime)
        {
            for(int i = 0; i< signals.Count; i++)
            {
                if(actualTime - signals[i].Time > ActualPeriod)
                {
                    signals.RemoveAt(i);
                    i--;
                }
            }
            UpdateTime = actualTime;
            return this;
        }

        public double AverageRssi
        {
            get
            {
                if(signals.Count > 0)
                {
                    double summ = 0.0;
                    foreach (var s in signals)
                    {
                        summ += (double)s.Rssi;
                    }

                    return (double)(summ / (double)signals.Count);
                }
                else
                {
                    return -999;
                }
            }
        }
        public int AmountRssi
        {
            get
            {
                if(signals.Count>0)
                {
                    int sum = 0;
                    foreach (var s in signals)
                    {
                        sum += s.Rssi;
                    }
                    return sum;
                }
                else
                {
                    return -999;
                }

               
            }
        }
    }
}
