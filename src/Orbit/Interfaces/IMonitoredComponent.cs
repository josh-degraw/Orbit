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
    public interface IMonitoredComponent<T> : IModuleComponent
        where T : class, IBoundedReport
    {
        /// <summary>
        /// Get the limit for the reports
        /// </summary>
        /// <returns> The limits for each report </returns>
        Task<Limit> GetComponentValueLimitAsync();

        /// <summary>
        /// Get the latest available report, returned as the explicit type.
        /// </summary>
        ValueTask<T?> GetLatestReportAsync();

        /// <summary>
        /// Returns an enumeration of reports as the concrete type of the report. Can be utilized if a component
        /// retrieves more than one kind of data.
        /// </summary>
        IAsyncEnumerable<T> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
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
        ValueTask<IBoundedReport?> GetLatestReportAsync();

        /// <summary>
        /// Returns an enumeration of reports. Can be utilized if a component retrieves more than one kind of data.
        /// </summary>
        IAsyncEnumerable<IBoundedReport> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }
}