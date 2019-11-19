using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Model
{
    public  class WasteWaterStorageTank
    {
        public Guid ID { get; set; }

        /// <summary>
        /// Current level of the wastewater collection tank
        /// recieves output from urine processing system and other wastewater sources (hygiene...)
        /// </summary>
        public int Level { get; set; }
    }
}
