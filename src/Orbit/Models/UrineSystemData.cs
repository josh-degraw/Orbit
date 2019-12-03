using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class UrineSystemData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

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
        public double UrineTankLevel { get; set; }

        /// <summary>
        /// status of pump assembly used to pull fluid from urine tank to the distiller assembly then from distiller
        /// assembly to the brine tank and water processor
        /// </summary>
        public string FluidControlPump { get; set; }

        /// <summary>
        /// number of rotations/minute
        /// </summary>
        public int DistillerSpeed { get; set; }

        public double DistillerTemp { get; set; }

        /// <summary>
        /// routes distillate and gasses from distiller to gas/liquid seperator cooled assembly aids condensation of
        /// water from gas
        /// </summary>
        public string PurgePump { get; set; }

        /// <summary>
        /// stores concentrated minerals and contaminates from urine distillation process for later disposal
        /// </summary>
        public int BrineTankLevel { get; set; }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            // Example:

            if (DistillerSpeed > 5000)
                yield return new Alert(nameof(DistillerSpeed), "DistillerSpeed too high", AlertLevel.HighWarning);
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "UrineSystem";

        #endregion Implementation of IModuleComponent
    }
}