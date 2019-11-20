using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    /// <summary>
    /// current system for ISS has changed to testing a mineral 'sponge' (zeolite) that absorbs CO2 when cold, then releases
    /// it when heated or exposed to a vacuum (space. Another system being developed involves algae. For simplicity, this 
    /// class is based on using the zeolite system.
    /// </summary>
    public class CarbonDioxideRemediation: IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "CarbonDioxideRemediation";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// whether the system is in absorb or release states
        /// </summary>
        public string CarbonDioxideRemoverStatus { get; set; }

        /// <summary>
        /// heater temp when zeolite is being 'replenished'
        /// </summary>
        public double Temperature { get; set; }

        public IEnumerable<Alert> GenerateAlerts()
        {
            yield break;
        }
    }
}
