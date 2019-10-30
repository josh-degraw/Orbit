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
    public interface IEventMonitor
    {
        void Start();

        void Stop();

        event EventHandler? Started;

        event EventHandler? Stopped;

        event EventHandler<CurrentValueReport>? NewValueRead;

        event EventHandler<ValueOutOfSafeRangeEventArgs>? ValueOutOfSafeRange;
    }

    public sealed class EventMonitor : IEventMonitor, IDisposable
    {
        // TODO: Determine an appropriate wait period

        private const double SecondsDelay = 2.0;

        private Task? _eventThread;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #region Singleton pattern

        private EventMonitor() => AppDomain.CurrentDomain.ProcessExit += (s, e) => this._cancellationTokenSource.Cancel();

        private static readonly Lazy<EventMonitor> _instance = new Lazy<EventMonitor>(() => new EventMonitor());

        public static IEventMonitor Instance => _instance.Value;

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

        public event EventHandler? Stopped;

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

        /// <summary>
        /// This method iterates through all known report types and generates alerts for them. It is then up to the
        /// consumer of said alerts as to how the alerts will be handled.
        /// </summary>
        /// <returns>  </returns>
        private async Task IterateReportedValues()
        {
            this.Started?.Invoke(this, EventArgs.Empty);
            while (!this._cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    foreach (Type reportType in this.ReportTypes)
                    {
                        using IServiceScope scope = OrbitServiceProvider.Instance.CreateScope();
                        Type componentType = this._componentByReportType.GetOrAdd(reportType, ExplicitlyMappedComponent);

                        var component = (IModuleComponent)scope.ServiceProvider.GetRequiredService(componentType);

                        if (this._cancellationTokenSource.IsCancellationRequested)
                            break;

                        // A component could generate more than one report, so those will be looped through here
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
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

        public void Start()
        {
            if (this._eventThread == null)
            {
                this._eventThread = Task.Run(this.IterateReportedValues, _cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                this._cancellationTokenSource.Cancel();
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