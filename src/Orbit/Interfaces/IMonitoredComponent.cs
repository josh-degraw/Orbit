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
        Task<Limit> GetComponentValueLimitAsync();

        Task<T?> GetLatestReportAsync();
        IAsyncEnumerable<T> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);

    }
}