using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trigger.Interfaces;

namespace Trigger.Signal
{
    public class TelemetryGroup : IEnumerable<Telemetry>, IObservable<Telemetry>
    {
        private class Unsubscriber : IDisposable
        {
            private IList<IObserver<Telemetry>> _observers;
            private IObserver<Telemetry> _observer;

            public Unsubscriber(IList<IObserver<Telemetry>> observers, IObserver<Telemetry> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }

        private IList<IObserver<Telemetry>> _observers = new List<IObserver<Telemetry>>();
        
        private IList<Telemetry> _items = new List<Telemetry>();

        public Telemetry this[string userId]
        {
            get
            {
                return _items.FirstOrDefault(x => string.Equals(x.UserId, userId, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        internal TelemetryGroup(IList<Telemetry> items)
        {
            _items = items;
        }

        public IEnumerator<Telemetry> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public IDisposable Subscribe(IObserver<Telemetry> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);

            return new Unsubscriber(_observers, observer);
        }

        public string Protocol
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var userTelemetry in this)
                {
                    sb.AppendLine(userTelemetry.Protocol);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
        }

        public void ProduceEvents()
        {
            foreach (var telemetry in this)
                foreach (var observer in _observers)
                {
                    /*if (!loc.HasValue)
                        observer.OnError(new LocationUnknownException());
                    else*/
                    observer.OnNext(telemetry);
                }
        }
    }
}