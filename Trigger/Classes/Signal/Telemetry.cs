using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Classes;
using Trigger.Enums;
using Trigger.Classes.Beacons;
using Trigger.Beacons;

namespace Trigger.Signal
{
    public sealed class Telemetry : ICollection<BeaconData>
    {
        private IList<BeaconData> _items = new List<BeaconData>();

        [JsonProperty(Order = 1)]
        public string UserId { get; set; }

        public DateTime? LastTimestamp
        {
            get
            {
                if (!_items.Any() || !_items.First().Any())
                    return null;

                return _items.SelectMany(b => b.Select(bi => bi.Time)).Max();
            }
        }

        public bool Contains(string address)
        {
            return _items.Any(x => string.Equals(x.Address, address, StringComparison.InvariantCultureIgnoreCase));
        }

        public BeaconData this[string address]
        {
            get
            {
                return _items.FirstOrDefault(x => string.Equals(x.Address, address, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public Telemetry Clone()
        {
            return new Telemetry
            {
                UserId = this.UserId,
                _items = this._items
            };
        }

        public void Add(BeaconData beacon)
        {
            var existedPoint = this[beacon.Address];

            if (existedPoint != null)
                existedPoint.Append(beacon);
            else _items.Add(beacon);
        }

        public Telemetry Append(BeaconData beacon)
        {
            var existedPoint = this[beacon.Address];

            if (existedPoint != null)
                existedPoint.Append(beacon);
            else _items.Add(beacon);
            return this;
        }

        public TelemetryType Type { get; set; } = TelemetryType.FromUser;

        public DateTime MinDateTime
        {
            get
            {
                return _items.SelectMany((b => b.Select(bi => bi.Time))).Min();
            }
        }

        public void Append(Telemetry telemetry)
        {
            if (!string.Equals(telemetry.UserId, UserId, StringComparison.CurrentCulture))
                return;

            foreach (var beacon in telemetry)
            {
                var b = this[beacon.Address];
                if (b != null)
                    b.Append(beacon);
                else _items.Add(beacon);
            }
        }

        public static Telemetry EmptyForUser(string userId)
        {
            return new Telemetry
            {
                Type = TelemetryType.FromUser,
                UserId = userId
            };
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(BeaconData item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(BeaconData[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(BeaconData item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<BeaconData> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public static implicit operator Telemetry(string s)
        {
            return JsonConvert.DeserializeObject<Telemetry>(s, new TelemetryJsonConverter());
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new TelemetryJsonConverter());
        }

        public string Protocol
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("Time$");
                foreach (var beaconData in _items)
                    sb.Append($"{beaconData.Address}$");

                sb.AppendLine();

                IEnumerable<DateTime> timeLines = _items.SelectMany(bi => bi.Select(x=>x.Time)).Distinct().OrderBy(x => x); //apointData.Beacons.SelectMany(b => b.Select(bi => bi.Time)).Distinct().OrderBy(x => x);
                DateTime minTime = timeLines.Min();

                foreach (var time in timeLines)
                {
                    sb.Append($"{Math.Round((time - minTime).TotalSeconds, 2)}$");

                    foreach (var beaconData in _items)
                    {
                        var bItem = beaconData.FirstOrDefault(bi => bi.Time == time);
                        sb.Append($"{(bItem.Rssi != 0 ? bItem.Rssi.ToString() : "")}$");
                    }

                    sb.AppendLine();
                }

                sb.AppendLine();

                return sb.ToString();
            }
        }

        public int Count => _items.Count;

        public bool IsReadOnly => _items.IsReadOnly;


        public Telemetry this[IEnumerable<IBeaconBody> beacons]
        {
            get
            {
                Telemetry result = this.Clone();
                result._items = _items.Where(b => beacons.Select(body => body.Address.ToLower()).Contains(b.Address.ToLower())).ToList();

                return result;
            }
        }
    }
}