using System.Threading.Tasks;
using Orbit.Models;
using Orbit.Util;

namespace Orbit
{
    /// <summary>
    /// Indicates a component that will be monitored regularly via <see cref="EventMonitor"/>
    /// </summary>
    public interface IMonitoredComponent: IModuleComponent
    {
        Task<BoundedValue> GetCurrentValueAsync();
    }
}