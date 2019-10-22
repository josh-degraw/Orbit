using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Orbit.Models;

namespace Orbit.Util
{
    public class EventMonitor
    {
        // TODO: Determine an appropriate wait period
        private const int MS_WAIT = 5000;
        private Task? _eventThread;

        #region Singleton pattern

        private static readonly Lazy<EventMonitor> _instance = new Lazy<EventMonitor>(() => new EventMonitor());
        
        public static EventMonitor Instance => _instance.Value;

        #endregion Singleton pattern

        private readonly ICollection<Type> _providerTypes = new HashSet<Type>();

        public EventHandler? Started;

        public EventHandler<ValueOutOfSafeRangeEventArgs>? ValueOutOfSafeRange;

        private IEnumerable<IMonitoredComponent> GetComponents()
        {
            using IServiceScope scope = OrbitServiceProvider.Instance.CreateScope();

            foreach (Type type in this._providerTypes)
            {
                var valProvider = (IMonitoredComponent)scope.ServiceProvider.GetRequiredService(type);
                yield return valProvider;
            }
        }
        
        private async Task WorkerMethodAsync()
        {
            this.Started?.Invoke(this, EventArgs.Empty);
            while (true)
            {
                foreach (IMonitoredComponent provider in this.GetComponents())
                {
                    BoundedValue val = await provider.GetCurrentValueAsync().ConfigureAwait(true);
                    if (!val.IsSafe)
                    {
                        this.ValueOutOfSafeRange?.Invoke(provider, new ValueOutOfSafeRangeEventArgs(provider.ComponentName, val));
                    }
                }

                await Task.Delay(MS_WAIT);
            }
        }

        public void Register(Type providerType)
        {
            if (!providerType.GetInterfaces().Contains(typeof(IMonitoredComponent)))
            {
                throw new InvalidOperationException("Types registered with EventMonitor must implement " + nameof(IMonitoredComponent));
            }
            this._providerTypes.Add(providerType);
        }

        public void Start()
        {
            if(_eventThread== null)
            {
                this._eventThread = Task.Run(this.WorkerMethodAsync);
            }
        }
    }
}