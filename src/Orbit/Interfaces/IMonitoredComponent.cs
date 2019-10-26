using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orbit.Models;
using Orbit.Util;

namespace Orbit
{
    public interface IMonitoredComponent :IModuleComponent
    {
        Task<double> GetCurrentValueAsync();
    }

    /// <summary>
    /// Indicates a component that will be monitored regularly via <see cref="EventMonitor"/>
    /// </summary>
    public interface IMonitoredComponent<out T> : IMonitoredComponent 
        where T : class, IBoundedReport
    {
        IAsyncEnumerable<T> GetReportsAsync(int? maxResults = 10, CancellationToken cancellationToken = default);
    }
}