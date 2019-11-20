using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Orbit.Models;
using Orbit.Util;

namespace Orbit
{
    /// <summary>
    /// Indicates a component that will be monitored regularly via <see cref="EventMonitor"/>
    /// </summary>
    public interface IMonitoredComponent<T> : IMonitoredComponent
        where T : class, IModel
    {
        /// <summary>
        /// Get the latest available report, returned as the explicit type.
        /// </summary>
        new ValueTask<T?> GetLatestReportAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumeration of reports as the concrete type of the report. Can be utilized if a component
        /// retrieves more than one kind of data.
        /// </summary>
        new ValueTask<IReadOnlyCollection<T>> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Non-generic version of <see cref="IMonitoredComponent{T}"/>, used by <see cref="EventMonitor"/> to avoid it
    /// needing to know the concrete report type.
    /// </summary>
    public interface IMonitoredComponent : IModuleComponent
    {
        /// <summary>
        /// Get the latest available report.
        /// </summary>
        /// <returns> </returns>
        ValueTask<IModel?> GetLatestReportAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns an enumeration of reports. Can be utilized if a component retrieves more than one kind of data.
        /// </summary>
        ValueTask<IReadOnlyCollection<IModel>> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }
}