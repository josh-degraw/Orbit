using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public  class WasteWaterStorageTank
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }
        
        /// <summary>
        /// distinguish between seperate tanks ('Tank 1', 'Tank 2')
        /// </summary>
        public string TankId { get; set; }

        /// <summary>
        /// Current level of the wastewater collection tank
        /// recieves output from urine processing system and other wastewater sources (hygiene...)
        /// </summary>
        public int Level { get; set; }
    }
}
