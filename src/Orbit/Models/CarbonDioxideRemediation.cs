using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public class CarbonDioxideRemediation
    {
        /// <summary>
        /// current system for ISS has changed to testing a mineral 'sponge' (zeolite) that absorbs CO2 when cold, then releases
        /// it when heated or exposed to a vacuum (space. Another system being developed involves algae. For simplicity, this 
        /// class is based on using the zeolite system.
        /// </summary>
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// whether the system is in absorb or release states
        /// </summary>
        public string CarbonDioxideRemoverStatus { get; set; }

        /// <summary>
        /// heater temp when zeolite is being 'replenished'
        /// </summary>
        public double Temperature { get; set; }

    }
}
