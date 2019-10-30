using System.Collections.Generic;
using System.Threading;
using Orbit.Models;

namespace Orbit
{
    /// <summary>
    /// Indicates a component that is able to generate snapshot reports of its values.
    /// </summary>
    public interface IReportableComponent
    {
        /// <summary>
        /// Generate one or more informational reports for a component.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        IAsyncEnumerable<CurrentValueReport> BuildCurrentValueReport(CancellationToken cancellationToken = default);
    }
}