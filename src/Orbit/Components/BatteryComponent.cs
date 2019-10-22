using System.Threading.Tasks;
using Orbit.Data;
using Orbit.Models;

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
            await Task.CompletedTask.ConfigureAwait(false);
            return default;
        }
    
        #endregion Implementation of IMonitoredComponent
    }
}