using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;

namespace Trigger.Classes.Beacons
{
    public class BeaconData : IList<BeaconItem>
    {
        public MacAddress Address { get; set; }
        private readonly IList<BeaconItem> _items;

        public BeaconData()
        {
            _items = new List<BeaconItem>();
        }

        public BeaconItem LastItem => this.OrderByDescending(i => i.Time).FirstOrDefault();

        public int Count => _items.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public BeaconItem this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static BeaconData FromMac(string mac)
        {
            return new BeaconData
            {
                Address = mac
            };
        }

        public void Append(BeaconData beacon)
        {
            if (beacon == null || beacon.Address != Address)
                return;

            foreach (var i in beacon)
                Add(i);
        }

        public void CleanBefore(DateTime time)
        {
            foreach (var item in this.Where(i => i.Time < time))
                Remove(item);
        }

        public int IndexOf(BeaconItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, BeaconItem item)
        {
            _items.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public void Add(BeaconItem item)
        {
            if (!this.Any(i => i.Time == item.Time))
                _items.Add(item);
        }

        public BeaconData Add(params BeaconItem[] infoes)
        {
            foreach (var item in infoes)
            {
                Add(item);
            }

            return this;
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(BeaconItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(BeaconItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(BeaconItem item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<BeaconItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public override string ToString()
        {
            return $"{Address}: {_items.Count} event(s)";
        }
    }
}
