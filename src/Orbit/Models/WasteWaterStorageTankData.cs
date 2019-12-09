using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class WasteWaterStorageTankData : IAlertableModel
    {
        /// <summary>
        /// The name of the component.
        /// </summary>
        [NotMapped]
        public string ComponentName => "WasteWaterStorageTank";

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

        public void ProcessData(SystemStatus waterStatus, SystemStatus urineStatus)
        {
            if(waterStatus == SystemStatus.Processing)
            {
                if(Level >= 5)
                {
                    Level -= 5;
                }
                else
                {
                    Level = 0;
                }
            }
            if(urineStatus == SystemStatus.Processing)
            {
                if(Level <= 95)
                {
                    Level += 5;
                }
                else
                {
                    Level = 0;
                }
            }
        }

        // Sample helper method
        private IEnumerable<Alert> GetLevels()
        {
            if (Level >= 100)
            {
                yield return new Alert(nameof(Level), "Tank overflowing", AlertLevel.HighError);
            }
            else if (Level >= 70)
            {
                yield return new Alert(nameof(Level), "Water level high", AlertLevel.HighWarning);
            }
            else if (Level <= 0)
            {
                yield return new Alert(nameof(Level), "Water level very low", AlertLevel.HighError);
            }
            else if (Level < 5)
            {
                yield return new Alert(nameof(Level), "Water level low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(Level));
            }
        }

        private IEnumerable<Alert> GetOtherAlerts()
        {
            yield break; // Equivalent to an empty collection
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            // Example of how to enumerate helper methods
            return GetLevels().Concat(GetOtherAlerts()).Concat(GetOtherAlerts());
        }

        #region Equality members

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

        #endregion Equality members
    }
}