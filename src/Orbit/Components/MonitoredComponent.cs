using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Orbit.Data;
using Orbit.Models;
using Orbit.Util;

namespace Orbit.Components
{
    public class MonitoredComponent<T> : IMonitoredComponent<T>, IMonitoredComponent
        where T : class, IBoundedReport
    {
        private readonly Lazy<string> _componentName;

        public string ComponentName => _componentName.Value;

        protected OrbitDbContext Database { get; }

        public MonitoredComponent(OrbitDbContext db)
        {
            this.Database = db;
            _componentName = new Lazy<string>(() => this.Database.Set<T>().AsNoTracking().Select(r => r.ReportType).First());
        }

        async ValueTask<IBoundedReport?> IMonitoredComponent.GetLatestReportAsync() => await this.GetLatestReportAsync().ConfigureAwait(false);

        IAsyncEnumerable<IBoundedReport> IMonitoredComponent.GetReportsAsync(int? maxResults, CancellationToken cancellationToken)
        {
            return this.GetReportsAsync(maxResults, cancellationToken);
        }

        public virtual async IAsyncEnumerable<T> GetReportsAsync(int? maxResults = 10, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = this.Database.Set<T>();

            if (maxResults != null)
                query = query.Take(maxResults.Value);

            await foreach (T item in query.ToAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Asynchronously returns the latest available report of type <typeparamref name="T"/> available.
        /// </summary>
        public async ValueTask<T?> GetLatestReportAsync()
        {
            var set = this.Database.Set<T>();
            T? val = await set.AsNoTracking().LastOrDefaultAsync().ConfigureAwait(false);

            return val;
        }

        public Task<Limit> GetComponentValueLimitAsync() => this.Database.Set<T>().AsNoTracking().Include(r => r.Limit).Select(r => r.Limit!).FirstAsync();

        public virtual async IAsyncEnumerable<CurrentValueReport> BuildCurrentValueReport([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            Limit limit = await this.GetComponentValueLimitAsync().ConfigureAwait(false);
            T? report = await this.GetLatestReportAsync().ConfigureAwait(false);

            if (report == null)
                yield break;

            var boundedValue = BoundedValue.Create(report.CurrentValue, limit);

            yield return new CurrentValueReport(this.ComponentName, report, boundedValue);
        }
    }

    public class BatteryComponent : MonitoredComponent<BatteryReport>
    {
        public BatteryComponent(OrbitDbContext db) : base(db)
        {
        }

        public override async IAsyncEnumerable<BatteryReport> GetReportsAsync(int? maxResults = 10, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // This overridden implementation is EXACTLY equivalent to the base class implementation
            IQueryable<BatteryReport> query = Database.BatteryReports;

            if (maxResults != null)
                query = query.Take(maxResults.Value);

            await foreach (var item in query.ToAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }
    }
}