using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Classes;
using Trigger.Enums;

namespace Trigger.Signal
{
    public sealed class Telemetry : ICollection<AccessPointData>
    {
        private IList<AccessPointData> _items = new List<AccessPointData>();

        [JsonProperty(Order = 1)]
        public string UserId { get; set; }

        public DateTime? LastTimestamp
        {
            get
            {
                DateTime? result = null;

                foreach (var d in this)
                {
                    var unitTimespan = d.LastTimestamp;

                    result = unitTimespan.HasValue && (unitTimespan > result || !result.HasValue) ?
                        unitTimespan : result;
                }

                return result;
            }
        }

        public bool Contains(string pointUid)
        {
            return _items.Any(x => string.Equals(x.AccessPointUid, pointUid, StringComparison.InvariantCultureIgnoreCase));
        }

        public AccessPointData this[string pointUid]
        {
            get
            {
                return _items.FirstOrDefault(x => string.Equals(x.AccessPointUid, pointUid, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        public void Add(AccessPointData point)
        {
            var existedPoint = this[point.AccessPointUid];

            if (existedPoint != null)
                existedPoint.Append(point);
            else _items.Add(point);
        }

        public TelemetryType Type { get; set; } = TelemetryType.FromUser;

        public DateTime MinDateTime
        {
            get
            {
                return _items.SelectMany(a => a.Beacons.SelectMany(b => b.Select(bi => bi.Time))).Min();
            }
        }

        public void Append(Telemetry telemetry)
        {
            if (!string.Equals(telemetry.UserId, UserId, StringComparison.CurrentCulture))
                return;

            foreach (var apoint in telemetry)
            {
                var point = this[apoint.AccessPointUid];
                if (point != null)
                    point.Append(apoint);
                else _items.Add(apoint);
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

        public bool Contains(AccessPointData item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(AccessPointData[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public bool Remove(AccessPointData item)
        {
            return _items.Remove(item);
        }

        public IEnumerator<AccessPointData> GetEnumerator()
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

        public string Protocol
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var apointData in this)
                {
                    sb.AppendLine($"Access point {apointData.AccessPointUid}:{Environment.NewLine}");

                    sb.AppendLine("Time");
                    foreach (var beacon in apointData.Beacons)
                        sb.Append($"\t{beacon.Address}");

                    sb.AppendLine();

                    IEnumerable<DateTime> timeLines = apointData.Beacons.SelectMany(b => b.Select(bi => bi.Time)).Distinct().OrderBy(x => x);
                    DateTime minTime = timeLines.Min();

                    foreach (var time in timeLines)
                    {
                        sb.Append($"{Math.Round((time - minTime).TotalSeconds, 2)}\t");

                        foreach (var beacon in apointData.Beacons)
                        {
                            var bItem = beacon.FirstOrDefault(bi => bi.Time == time);
                            sb.Append($"{(bItem.Rssi != 0 ? bItem.Rssi.ToString() : "")}\t");
                        }

                        sb.AppendLine();
                    }

                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }

        public int Count => _items.Count;

        public bool IsReadOnly => _items.IsReadOnly;
    }
}