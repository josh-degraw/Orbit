using System.Collections.Generic;
using System.Threading;
using Orbit.Models;

namespace Orbit
{
    public interface IReportableComponent
    {
        IAsyncEnumerable<(string ComponentName, BoundedValue Value)> BuildCurrentValueReport(CancellationToken cancellationToken = default);
    }
}