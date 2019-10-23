using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Orbit.Data;
using Orbit.Models;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Orbit.Components
{
    public class BatteryComponent : IMonitoredComponent
    {
        public string ComponentName => "Battery";

        private readonly OrbitDbContext _database;

        public BatteryComponent(OrbitDbContext db)
        {
            this._database = db;
        }

        #region Implementation of IMonitoredComponent

        public async Task<BoundedValue> GetCurrentValueAsync()
        {
            BatteryReport? val = await this._database.BatteryReports.Include(r => r.Limit).FirstOrDefaultAsync();

            if (val == null)
                throw new InvalidOperationException("No data retrieved");

            return BoundedValue.Create(val.CurrentValue, val.Limit);
        }

        #endregion Implementation of IMonitoredComponent
    }
}