using Microsoft.EntityFrameworkCore;

using Orbit.Data;
using Orbit.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Orbit.Components
{
    public class MonitoredComponent<T> : IMonitoredComponent<T>
        where T : class, IBoundedReport
    {
        private readonly Lazy<string> _componentName;

        public string ComponentName => _componentName.Value;

        private readonly OrbitDbContext _database;

        public MonitoredComponent(OrbitDbContext db)
        {
            this._database = db;
            _componentName =
                new Lazy<string>(() => _database.Set<T>().AsNoTracking().Select(r => r.ReportType).First());
        }

        #region Implementation of IMonitoredComponent

        public async IAsyncEnumerable<T> GetReportsAsync(int? maxResults = 10, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var query = _database.Set<T>().AsNoTracking();

            if (maxResults != null)
                query = query.Take(maxResults.Value);

            await foreach (T item in query.ToAsyncEnumerable().WithCancellation(cancellationToken))
            {
                yield return item;
            }
        }

        public async Task<double> GetCurrentValueAsync()
        {
            IBoundedReport? val = await this._database.Set<T>().AsNoTracking().FirstOrDefaultAsync();

            if (val == null)
                throw new InvalidOperationException("No data retrieved");

            return val.CurrentValue;
        }

        #endregion Implementation of IMonitoredComponent
    }
}