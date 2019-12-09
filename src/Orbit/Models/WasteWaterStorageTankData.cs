using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        /// distinguish between separate tanks ('Tank 1', 'Tank 2')
        /// </summary>
        public string TankId { get; set; }

        /// <summary>
        /// Current level of the waste water collection tank receives output from urine processing system and other
        /// wastewater sources (hygiene...)
        /// </summary>
        public double Level { get; set; }

        public void ProcessData(SystemStatus urineProcessor, SystemStatus waterProcessor)
        {
            if(urineProcessor == SystemStatus.Processing)
            {
                if(Level <= 96)
                {
                    Level += 4;
                }
                else
                {
                    Level = 0;
                }
            }
            if (waterProcessor == SystemStatus.Processing)
            {
                if (Level >= 6)
                {
                    Level -= 6;
                }
                else
                {
                    Level = 0;
                }
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()

        {
            if (this.Level >= 100)
            {
                yield return new Alert(nameof(this.Level), "Tank overflowing", AlertLevel.HighError);
            }
            else if (this.Level >= 70)
            {
                yield return new Alert(nameof(this.Level), "Water level high", AlertLevel.HighWarning);
            }
            else if (this.Level <= 0)
            {
                yield return new Alert(nameof(this.Level), "Water level very low", AlertLevel.HighError);
            }
            else if (this.Level < 5)
            {
                yield return new Alert(nameof(this.Level), "Water level low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(this.Level));
            }
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

        public override int GetHashCode() => HashCode.Combine(this.TankId, this.Level);

        #endregion Equality members
    }
}