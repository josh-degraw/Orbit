using Microsoft.Extensions.DependencyInjection;

using Orbit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Orbit.Util
{
    public sealed class EventMonitor : IDisposable
    {
        // TODO: Determine an appropriate wait period
        private const int MS_WAIT = 5000;

        private Task? _eventThread;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Singleton pattern

        private EventMonitor()
        {
            AppDomain.CurrentDomain.ProcessExit += (s, e) => _cancellationTokenSource.Cancel();
        }

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

        private IEnumerable<IReportableComponent> GetComponents()
        {
            foreach (Type type in this._providerTypes)
            {
                using IServiceScope scope = OrbitServiceProvider.Instance.CreateScope();

                var valProvider = (IReportableComponent)scope.ServiceProvider.GetRequiredService(type);
                yield return valProvider;
            }
        }

        private async Task WorkerMethodAsync()
        {
            this.Started?.Invoke(this, EventArgs.Empty);
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                foreach (IReportableComponent provider in this.GetComponents())
                {
                    if (_cancellationTokenSource.IsCancellationRequested)
                        break;

                    await foreach (var report in provider.BuildCurrentValueReport(_cancellationTokenSource.Token))
                    {
                        if (!report.Value.IsSafe)
                        {
                            this.ValueOutOfSafeRange?.Invoke(provider, report);
                        }
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

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this._cancellationTokenSource.Dispose();
            this._eventThread?.Dispose();
        }

        #endregion Implementation of IDisposable
    }
}