using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orbit.Models
{
    public enum BedOptions
    {
        Bed1,
        Bed2
    }
    /// <summary>
    /// current system for ISS has changed to testing a mineral 'sponge' (zeolite) that absorbs CO2 when cold, then
    /// releases it when heated or exposed to a vacuum (space. Another system being developed involves algae. For
    /// simplicity, this class is based on using the zeolite system.
    /// </summary>
   
    public class CarbonDioxideRemediation : IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "CarbonDioxideRemediation";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// current operating state of the system
        /// </summary>
        public SystemStatus Status { get; set; }
        
        /// <summary>
        /// Circulation fan to move air over carbon dioxide absorbing zeolite beds
        /// </summary>
        public bool FanOn { get; set; }

        /// <summary>
        /// determines bed that airflow will pass through
        /// </summary>
        public BedOptions BedSelectorValve { get; set; }

        /// <summary>
        /// specifies which bed is actively absorbing carbon dioxide
        /// </summary>
        public BedOptions AbsorbingBed { get; set; }
        
        /// <summary>
        /// specifies which bed is releasing carbon dioxide
        /// </summary>
        public BedOptions RegeneratingBed { get; set; }

        /// <summary>
        /// heater temp when zeolite adsorption capability is being 'regenerated' 400F designed temp, 126.7C (260F) on
        /// ISS to conserve power
        /// </summary>
        [Range(0, 232)]
        public int Bed1Temperature { get; set; }

        [Range(0, 232)]
        public int Bed2Temperature { get; set; }

        /// <summary>
        /// the level of carbon dioxide in air exiting the co2 scrubber
        /// </summary>
        public double OutputCo2Level { get; set; }

        private IEnumerable<Alert> CheckRegenerationTemp(double temperature, string name)
        {
            if (temperature >= TemperatureUpperLimit)
            {
                yield return new Alert(nameof(CarbonDioxideRemediation), $"{name} temperature is above maximum", AlertLevel.HighError);
            }
            else if (temperature >= TemperatureElevated)
            {
                Trouble();
                return;
            }

            if (count < countLength)
            {
                count++;
            }
            else if (temperature <= TemperatureLow)
            {
                yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 1 temperature is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CarbonDioxideRemediation));
            }

        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckRegenerationTemp()
                .Concat(CheckFan())
                .Concat(CheckOutputCo2Level())
                .Concat(CheckBedsAlternate());
        }

        #endregion Check Alerts

        #region Equality Members

        public bool Equals(CarbonDioxideRemediation other)
        {
            if(ReferenceEquals(null, other))
            {
                return false;
            }
            if(ReferenceEquals(this, other))
            {
                return true;
            }
            return this.ReportDateTime.Equals(other.ReportDateTime)
                && this.Status == other.Status
                && this.FanOn == other.FanOn
                && this.BedSelectorValve == other.BedSelectorValve
                && this.AbsorbingBed == other.AbsorbingBed
                && this.RegeneratingBed == other.RegeneratingBed
                && this.Bed1Temperature == other.Bed1Temperature
                && this.Bed2Temperature == other.Bed2Temperature
                && this.OutputCo2Level == other.OutputCo2Level;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is CarbonDioxideRemediation other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.ReportDateTime,
                this.Status,
                this.FanOn,
                this.BedSelectorValve,
                this.AbsorbingBed,
                this.RegeneratingBed,
                this.Bed1Temperature,
                (this.Bed2Temperature, this.OutputCo2Level)
                );
        }

        public static bool operator ==(CarbonDioxideRemediation left, CarbonDioxideRemediation right) => Equals(left, right);
        public static bool operator !=(CarbonDioxideRemediation left, CarbonDioxideRemediation right) => !Equals(left, right);

        #endregion Equality Members
    }
}