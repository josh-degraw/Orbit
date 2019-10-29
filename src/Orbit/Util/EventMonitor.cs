using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

using Orbit.Components;
using Orbit.Models;

namespace Orbit.Util
{
    public sealed class EventMonitor : IDisposable
    {
        // TODO: Determine an appropriate wait period

        private const double SecondsDelay = 10.0;

        private Task? _eventThread;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Singleton pattern

        private EventMonitor() => AppDomain.CurrentDomain.ProcessExit += (s, e) => this._cancellationTokenSource.Cancel();

        private static readonly Lazy<EventMonitor> _instance = new Lazy<EventMonitor>(() => new EventMonitor());

        public static EventMonitor Instance => _instance.Value;

        #endregion Singleton pattern

        /// <summary>
        /// Returns a collection of the Report types defined in this assembly.
        /// </summary>
        private IEnumerable<Type> ReportTypes => this._reportTypes.Value;

        private readonly Lazy<ICollection<Type>> _reportTypes =
            new Lazy<ICollection<Type>>(() =>
            {
                IEnumerable<Type> allTypes = Assembly.GetExecutingAssembly().ExportedTypes;
                return allTypes.Where(t => t.GetInterfaces().Contains(typeof(IBoundedReport))).ToList();
            });

        public event EventHandler? Started;

        public event EventHandler<CurrentValueReport>? NewValueRead;
        public event EventHandler<ValueOutOfSafeRangeEventArgs>? ValueOutOfSafeRange;

        private static Type ExplicitlyMappedComponent(Type reportType)
        {
            Type explicitlyDefined = Assembly.GetExecutingAssembly().ExportedTypes
                .SingleOrDefault(a => a.GetInterfaces().Contains(typeof(IBoundedReport))
                                      && a.GetGenericArguments().Contains(reportType));

            // If there exists a class explicitly defined to handle the given report, use that Otherwise, assume it only
            // generates one report and create a MonitoredComponent<T> of the given type to handle it

            return explicitlyDefined ?? typeof(MonitoredComponent<>).MakeGenericType(reportType);
        }

        private readonly ConcurrentDictionary<Type, Type> _componentByReportType = new ConcurrentDictionary<Type, Type>();

        private async Task WorkerMethodAsync()
        {
            this.Started?.Invoke(this, EventArgs.Empty);
            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                foreach (Type reportType in this.ReportTypes)
                {
                    using IServiceScope scope = OrbitServiceProvider.Instance.CreateScope();
                    Type componentType = this._componentByReportType.GetOrAdd(reportType, ExplicitlyMappedComponent);

                    var component = (IModuleComponent)scope.ServiceProvider.GetRequiredService(componentType);

                    if (this._cancellationTokenSource.IsCancellationRequested)
                        break;

                    await foreach (CurrentValueReport report in component.BuildCurrentValueReport(this._cancellationTokenSource.Token))
                    {
                        NewValueRead?.Invoke(component, report);

                        if (!report.Value.IsSafe)
                        {
                            this.ValueOutOfSafeRange?.Invoke(component, new ValueOutOfSafeRangeEventArgs(report));
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(SecondsDelay)).ConfigureAwait(false);
                }
            }
        }

        public void Start()
        {
            if (this._eventThread == null)
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