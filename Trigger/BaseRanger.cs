using System;
using System.Collections.Generic;
using System.Text;
using Trigger.Classes.Logging;
using Trigger.Enums;
using Trigger.Interfaces;
using Trigger.Signal;

namespace Trigger
{
    public abstract class BaseRanger : IRanger
    {
        public event EventHandler<TriggerEventArgs> OnEvent;
       
        
        public virtual bool IsObsolete() => false;
        internal ILogger _logger;
        internal string _spaceUid;
        protected IDisposable _unsubscriber;
        protected string _userUid { get; set; }

        public void OnCompleted()
        {
            _unsubscriber?.Dispose();
        }

        public void OnError(Exception error)
        {
            
        }

        public abstract void OnNext(Telemetry value);

        public void Subscribe(IObservable<Telemetry> provider)
        {
            _unsubscriber = provider.Subscribe(this);
        }

        protected void RaiseEvent(TriggerEventType type, DateTime time)
        {
            OnEvent?.Invoke(this, new TriggerEventArgs
            {
                SpaceUid = _spaceUid,
                Timespan = time,
                UserId = _userUid,
                Type = type
            });
        }

        public virtual string Report => string.Empty;

        
    }
}
