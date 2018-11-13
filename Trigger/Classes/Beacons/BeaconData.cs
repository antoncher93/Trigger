using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Trigger.Beacons;

namespace Trigger.Classes.Beacons
{
    public class BeaconData : IList<BeaconItem>
    {
        public string Address { get; set; }
        private readonly IList<BeaconItem> _items;

        public BeaconData()
        {
            _items = new List<BeaconItem>();
        }

        public BeaconItem LastItem => this.OrderByDescending(i => i.Time).FirstOrDefault();

        public int Count => _items.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        public BeaconItem this[int index] { get => _items[index]; set => _items[index] = value; }

        public static BeaconData FromAddress(string address)
        {
            return new BeaconData
            {
                Address = address
            };
        }

        public void Append(BeaconData beacon)
        {
            if (beacon == null || beacon.Address != Address)
                return;

            foreach (var i in beacon)
            {
                Add(i);
            }
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
