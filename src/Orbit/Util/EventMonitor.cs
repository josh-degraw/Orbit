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

        event EventHandler<ValueReadEventArgs>? NewValueRead;

        /// <summary>
        /// Triggered when any alert is reported for a newly read value.
        /// </summary>
        event EventHandler<AlertEventArgs>? AlertReported;

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
        private IReadOnlyCollection<Type> ReportTypes => this._reportTypes.Value;

        private readonly Lazy<IReadOnlyCollection<Type>> _reportTypes =
            new Lazy<IReadOnlyCollection<Type>>(() =>
            {
                IEnumerable<Type> allTypes = Assembly.GetExecutingAssembly().ExportedTypes;
                return allTypes.Where(t => t.GetInterfaces().Contains(typeof(IModel))).ToList();
            });

        /// <summary>
        /// Invoked when the event monitor thread starts.
        /// </summary>
        public event EventHandler? Started;

        /// <summary>
        /// Invoked when the event monitor thread stops.
        /// </summary>
        public event EventHandler? Stopped;

        /// <summary>
        /// Triggered for every newly returned point of data for each registered report.
        /// </summary>
        public event EventHandler<ValueReadEventArgs>? NewValueRead;

        /// <summary>
        /// Triggered when any alert is reported for a newly read value.
        /// </summary>
        public event EventHandler<AlertEventArgs>? AlertReported;
        
        private static Type ExplicitlyMappedComponent(Type reportType)
        {
            var explicitType = typeof(IMonitoredComponent<>).MakeGenericType(reportType);

            Type explicitlyDefined = Assembly
                .GetExecutingAssembly()
                .ExportedTypes
                .SingleOrDefault(a => a.GetInterfaces().Contains(explicitType));

            // If there exists a class explicitly defined to handle the given report, use that Otherwise, assume it only
            // generates one report and create a MonitoredComponent<T> of the given type to handle it

            return explicitlyDefined ?? typeof(MonitoredComponent<>).MakeGenericType(reportType);
        }

        private readonly ConcurrentDictionary<Type, Type> _componentByReportType = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// This method iterates through all known report types and generates alerts for them. It is then up to the
        /// consumer of said alerts as to how the alerts will be handled.
        /// </summary>
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

                        var component = (IMonitoredComponent) scope.ServiceProvider.GetRequiredService(componentType);

                        if (this._cancellationTokenSource.IsCancellationRequested)
                            break;
                        
                        foreach (var report in await component.GetReportsAsync())
                        {
                            NewValueRead?.Invoke(component, new ValueReadEventArgs(report));

                            if (report is IAlertableModel alertable)
                            {
                                foreach (var alert in alertable.GenerateAlerts())
                                {
                                    var args = new AlertEventArgs(alertable, alert);

                                    AlertReported?.Invoke(this, args);
                                }
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
                this._eventThread = Task.Run(this.IterateReportedValues, this._cancellationTokenSource.Token);
            }
        }

        public void Stop()
        {
            if (!this._cancellationTokenSource.IsCancellationRequested)
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

    public class ValueReadEventArgs : EventArgs
    {
        public ValueReadEventArgs(IModel report)
        {
            this.Report = report;
        }

        public IModel Report { get; }
    }


    public class AlertEventArgs : ValueReadEventArgs
    {
        public AlertEventArgs(IModel report, Alert alert) : base(report)
        {
            this.Alert = alert;
        }
        
        public Alert Alert { get; }
    }
}