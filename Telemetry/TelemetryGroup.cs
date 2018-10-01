using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trigger.Telemetry
{
    public class TelemetryGroup : ICollection<Telemetry>
    {
        private IList<Telemetry> _telemetries = new List<Telemetry>();

        public int Count => _telemetries.Count;

        public bool IsReadOnly => _telemetries.IsReadOnly;

        public void Add(Telemetry item)
        {
            Telemetry res = _telemetries.FirstOrDefault(t => string.Equals(item.Data.UserId, t.Data.UserId));
            if(res == null)
                _telemetries.Add(item);
            else
                res.Append(item);
        }

        public void Clear()
            => _telemetries.Clear();

        public bool Contains(Telemetry item)
            => _telemetries.Contains(item);

        public void CopyTo(Telemetry[] array, int arrayIndex)
            => _telemetries.CopyTo(array, arrayIndex);

        public IEnumerator<Telemetry> GetEnumerator()
            => _telemetries.GetEnumerator();

        public bool Remove(Telemetry item)
            => _telemetries.Remove(item);

        IEnumerator IEnumerable.GetEnumerator()
            => _telemetries.GetEnumerator();
    }
}