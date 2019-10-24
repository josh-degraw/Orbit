using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Orbit.Models;

namespace Orbit.Components
{
    public abstract class SingleValueModuleComponentBase : IMonitoredComponent, IReportableComponent
    {
        protected SingleValueModuleComponentBase()
        {
            
        }
        public abstract string ComponentName { get; }

        public abstract Task<Limit> GetComponentLimitsAsync();

        public abstract Task<double> GetCurrentValueAsync();

        public virtual async IAsyncEnumerable<(string ComponentName, BoundedValue Value)> BuildCurrentValueReport([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Limit limit = await this.GetComponentLimitsAsync();
            double val = await this.GetCurrentValueAsync().ConfigureAwait(true);
            BoundedValue boundedValue = BoundedValue.Create(val, limit);

            yield return (this.ComponentName, boundedValue);
        }
    }
}