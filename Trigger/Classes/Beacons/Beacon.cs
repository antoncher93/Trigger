using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;

namespace Trigger.Classes.Beacons
{
    public class Beacon : IList<BeaconItem>
    {
        public string Mac { get; set; }
        private readonly IList<BeaconItem> _items;

        public Beacon()
        {
            _items = new List<BeaconItem>();
        }

        public BeaconItem LastItem => this.OrderByDescending(i => i.Time).FirstOrDefault();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public BeaconItem this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static Beacon FromMac(string mac)
        {
            return new Beacon
            {
                Mac = mac
            };
        }

        public void Append(Beacon beacon)
        {
            if (beacon == null || beacon.Mac != Mac)
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

        public Beacon Add(params BeaconItem[] infoes)
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
    }
}
