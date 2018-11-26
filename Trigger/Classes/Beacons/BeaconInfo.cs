using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Trigger.Classes;
using Trigger.Classes.Beacons;
using Trigger.Classes.Logging;

namespace Trigger.Beacons
{
    public class BeaconInfo : ICollection<BeaconItem>
    {
        private IList<BeaconItem> _signals = new List<BeaconItem>();
        public string MacAddress { get; private set; }
        private ILogger _logger;
        public TimeSpan ActualPeriod { get; set; }
        public int TxPower { get; set; } = -40;
        public double BaseX { get; set; } = 0;
        public double BaseY { get; set; } = 0;
        public BeaconItem MaxSignal { get; private set; } = BeaconItem.Default;

        public double AverageRssi
        {
            get
            {
                if(_signals.Count>0)
                {
                    double summ = 0.0;
                    foreach (var s in _signals)
                    {
                        summ += s.Rssi;
                    }
                    return summ / (double)_signals.Count;
                }

                return double.MinValue;
            }
        }

        public double Distance => Calculator.BeaconDiastance(AverageRssi, TxPower);

        private string _rssi_to_set = "";

        public int Count => _signals.Count;

        public bool IsReadOnly => _signals.IsReadOnly;

        public BeaconInfo(string mac, int milliseconds)
        {
            MacAddress = mac;
            ActualPeriod = TimeSpan.FromMilliseconds(milliseconds);
        }

        public BeaconInfo(string mac, int milliseconds, ILogger logger)
        {
            MacAddress = mac;
            ActualPeriod = TimeSpan.FromMilliseconds(milliseconds);
            _logger = logger;
        }

        public void Add(BeaconItem item)
        {
            _rssi_to_set = item.Rssi.ToString();

            _signals.Add(item);
            Update(item.Time);

            var f = MaxSignal.Equals(BeaconItem.Default);

            if (MaxSignal.Equals(BeaconItem.Default) || item.Rssi > MaxSignal.Rssi)
            {
                MaxSignal = item;
            }
        }

        public void Update(DateTime actualTime)
        {
            for (int i = 0; i < _signals.Count; i++)
            {
                if (actualTime - _signals[i].Time > ActualPeriod)
                {
                    _signals.RemoveAt(i);
                    i--;
                }
            }

            _logger?.Log(new string[] {actualTime.TimeOfDay.ToString(), MacAddress, _rssi_to_set,  AverageRssi.ToString(), Distance.ToString()});
            _rssi_to_set = "";
        }

        #region ICollection

        public void Clear()
        {
            _signals.Clear();
        }

        public bool Contains(BeaconItem item)
        {
            return _signals.Contains(item);
        }

        public void CopyTo(BeaconItem[] array, int arrayIndex)
        {
            _signals.CopyTo(array, arrayIndex);
        }

        public bool Remove(BeaconItem item)
        {
            return _signals.Remove(item);
        }

        public IEnumerator<BeaconItem> GetEnumerator()
        {
            return _signals.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _signals.GetEnumerator();
        }

#endregion


    }


}
