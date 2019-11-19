using System;
using System.Collections.Generic;
using System.Text;

namespace Orbit.Models
{
    public class UrineProcessor
    {
        public Guid ID { get; set; }
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// distinguish between seperate redundant processors ('MainProcessor', 'Backup 1')
        /// </summary>
        public string ProcessorId { get; set; }

        /// <summary>
        /// indicator of overall system status (Ready, Processing, Failure...)
        /// </summary>
        public string SystemStatus { get; set; }

        /// <summary>
        /// Percentage full of treated urine holding tank
        /// </summary>
        public int UrineTankLevel { get; set; }

        /// <summary>
        /// status of pump assembly used to pull fluid from urine tank to the distiller assembly 
        /// then from distiller assembly to the brine tank and water processor
        /// </summary>
        public string FluidControlPump { get; set; }
        /// <summary>
        /// number of rotations/minute
        /// </summary>
        public int DistillerSpeed { get; set; }
        public double DistillerTemp { get; set; }

        /// <summary>
        /// routes distillate and gasses from distiller to gas/liquid seperator
        /// cooled assembly aids condensation of water from gas
        /// </summary>
        public string PurgePump { get; set; }
        
        /// <summary>
        ///  stores concentrated minerals and contaminates from urine distillation process for later disposal
        /// </summary>
        public int BrineTankLevel { get; set; }
        
    }
   
}
