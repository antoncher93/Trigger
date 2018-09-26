using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trigger.Telemetry
{
    public class TelemetryGroup : ICollection<Telemetry>
    {
        private ICollection<Telemetry> _telemetries;

        public int Count => _telemetries.Count;

        public bool IsReadOnly => _telemetries.IsReadOnly;

        public void Add(Telemetry item)
        {
            Telemetry res = _telemetries.FirstOrDefault(t => string.Equals(item.Data.UserId, t.Data.UserId));
            if(res == null)
            {
                _telemetries.Add(item);
            }
            else
            {

            }
        }

        public void Clear()
        {
            _telemetries.Clear();
        }

        public bool Contains(Telemetry item)
        {
            return _telemetries.Contains(item);
        }

        public void CopyTo(Telemetry[] array, int arrayIndex)
        {
            
        }

        public IEnumerator<Telemetry> GetEnumerator()
        {
            return _telemetries.GetEnumerator();
        }

        public bool Remove(Telemetry item)
        {
            return _telemetries.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _telemetries.GetEnumerator();
        }
    }
}
