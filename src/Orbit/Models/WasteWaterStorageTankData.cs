using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public class WasteWaterStorageTankData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// distinguish between seperate tanks ('Tank 1', 'Tank 2')
        /// </summary>
        public string TankId { get; set; }

        /// <summary>
        /// Current level of the wastewater collection tank recieves output from urine processing system and other
        /// wastewater sources (hygiene...)
        /// </summary>
        public double Level { get; set; }

        public IEnumerable<Alert> GenerateAlerts()
        {
            //TODO: Implement
            yield break;
        }

        #region Implementation of IModuleComponent

        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "WasteWaterStorageTank";

        #endregion Implementation of IModuleComponent
    }
}