using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Orbit.Models;
using Orbit.Util;

namespace Orbit.Components
{
    /// <summary>
    /// Represents a component that reports a one or more values including, but not limited to, the specified <typeparamref name="T"/>.
    /// For example, a battery component that reports temperature as well as battery heat.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReportableComponent<T> : IReportableComponent where T : class, IBoundedReport
    {
        private readonly IMonitoredComponent<T> _component;

        public ReportableComponent(IMonitoredComponent<T> component)
        {
            this._component = component;
        }

        public virtual async IAsyncEnumerable<CurrentValueReport> BuildCurrentValueReport([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Limit limit = await this._component.GetComponentValueLimitAsync().ConfigureAwait(false);
            T? report = await this._component.GetLatestReportAsync().ConfigureAwait(false);
            
            if (report == null)
                throw Exceptions.NoDataFound();

            var boundedValue = BoundedValue.Create(report.CurrentValue, limit);

            yield return new CurrentValueReport(this._component.ComponentName, report, boundedValue);
        }

    }
}