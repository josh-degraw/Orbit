using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private ICollection<Type> _providerTypes => _reportType.Value;


        private readonly Lazy<ICollection<Type>> _reportType = 
            new Lazy<ICollection<Type>>(() =>
            {
                var allTypes = Assembly.GetExecutingAssembly().ExportedTypes;
                return allTypes.Where(t => t.GetInterfaces().Contains(typeof(IBoundedReport))).ToList();
            });

        public EventHandler? Started;

        public EventHandler<ValueOutOfSafeRangeEventArgs>? ValueOutOfSafeRange;

        private IEnumerable<IMonitoredComponent> GetComponents()
        {
            foreach (Type type in this._providerTypes)
            {
                using IServiceScope scope = OrbitServiceProvider.Instance.CreateScope();

                var valProvider = (IMonitoredComponent)scope.ServiceProvider.GetRequiredService(type);
                yield return valProvider;
            }
        }

        private async Task WorkerMethodAsync()
        {
            this.Started?.Invoke(this, EventArgs.Empty);
            while (true)
            {
                foreach (IMonitoredComponent component in this.GetComponents())
                {
                    BoundedValue val = await component.GetCurrentValueAsync().ConfigureAwait(true);
                    if (!val.IsSafe)
                    {
                        this.ValueOutOfSafeRange?.Invoke(component, new ValueOutOfSafeRangeEventArgs(component.ComponentName, val));
                    }
                }

                await Task.Delay(MS_WAIT);
            }
        }

        public void Start()
        {
            if (_eventThread == null)
            {
                this._eventThread = Task.Run(this.WorkerMethodAsync);
            }
        }
    }
}