using System;
using System.Collections.Generic;
using System.Threading;

namespace Orbit.Util
{
    public class EventMonitor
    {
        private static readonly Lazy<EventMonitor> _instance = new Lazy<EventMonitor>(() => new EventMonitor());
        private const int MS_WAIT = 5000;
        private readonly Thread _eventThread;

        private EventMonitor()
        {
            this._eventThread = new Thread(this.WorkerMethod);
            this._eventThread.Start();
        }

        private readonly ICollection<IValueProvider> _valueProviders = new List<IValueProvider>();

        public static EventMonitor Instance => _instance.Value;

        public EventHandler<ValueOutOfSafeRangeEventArgs> ValueOutOfSafeRange;

        private void RunChecks()
        {
            foreach (var provider in this._valueProviders)
            {
                var val = provider.GetCurrentValue();
                if (!val.IsSafe)
                {
                    this.ValueOutOfSafeRange?.Invoke(provider, new ValueOutOfSafeRangeEventArgs(val));
                }
            }
        }

        private void WorkerMethod()
        {
            while (true)
            {
                this.RunChecks();
                Thread.Sleep(MS_WAIT);
            }
        }
        #region Observation

        public IDisposable Subscribe(IValueProvider observer)
        {
            this._valueProviders.Add(observer);
            return new UnSubscriber(this._valueProviders, observer);
        }

        private sealed class UnSubscriber : IDisposable
        {
            private readonly ICollection<IValueProvider> _observers;
            private readonly IValueProvider _observer;

            public UnSubscriber(ICollection<IValueProvider> observers, IValueProvider observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (this._observers.Contains(this._observer))
                {
                    this._observers.Remove(this._observer);
                }
            }
        }

        #endregion Observation
    }
}