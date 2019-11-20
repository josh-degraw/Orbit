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

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            if (Level >= 100)
            {
                yield return new Alert(nameof(Level), "Waste Water storage tank overflowing", AlertLevel.HighError);
            }
            else if (Level >= 70)
            {
                yield return new Alert(nameof(Level), "Waste Water storage tank water level high", AlertLevel.HighWarning);
            }
            else if (Level <= 0)
            {
                yield return new Alert(nameof(Level), "Waste Water storage tank water level very low", AlertLevel.HighError);
            }
            else if (Level < 5)
            {
                yield return new Alert(nameof(Level), "Waste Water storage tank water level low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(Level));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is WasteWaterStorageTankData data)
            {
                return data.TankId == this.TankId && Math.Abs(data.Level - this.Level) < 0.001;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TankId, Level);
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