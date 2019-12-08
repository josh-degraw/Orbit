using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    /// <summary>
    /// current system for ISS has changed to testing a mineral 'sponge' (zeolite) that absorbs CO2 when cold, then releases
    /// it when heated or exposed to a vacuum (space. Another system being developed involves algae. For simplicity, this 
    /// class is based on using the zeolite system.
    /// </summary>
    public class CarbonDioxideRemediation: IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "CarbonDioxideRemediation";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// false when the system is in absorb state, true when system is in release state 
        /// </summary>
        public bool Bed1Regenerating { get; set; }
        public bool Bed2Regenerating { get; set; }

        /// <summary>
        /// heater temp when zeolite adsorption capability is being 'regenerated'
        /// 400F designed temp, 126.7C (260F) on ISS to conserve power
        /// </summary>
        [Range(0, 232)]
        public double Bed1Temperature { get; set; }
        [Range(0, 232)]
        public double Bed2Temperature { get; set; }


        [NotMapped]
        public double TemperatureUpperLimit = 133;
        [NotMapped]
        public double TemperatureLowerLimit = 120;

        private IEnumerable<Alert> CheckRegenerationHeaterTemp()
        {
            if (Bed1Regenerating)
            {
                if (Bed1Temperature >= TemperatureUpperLimit)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 1 temperature is above maximum", AlertLevel.HighError);
                }
                else if(Bed1Temperature >= 127)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 1 temperature is elevated", AlertLevel.HighWarning);
                }
                else if(Bed1Temperature <= TemperatureLowerLimit)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 1 temperature is below minimum", AlertLevel.LowError);
                }
                else if(Bed1Temperature <= 123)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 1 temperature is too low", AlertLevel.LowWarning);
                }
                else
                {
                    yield return Alert.Safe(nameof(CarbonDioxideRemediation));
                }
            }
            else
            {
                if (Bed2Temperature >= TemperatureUpperLimit)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 2 temperature is above maximum", AlertLevel.HighError);
                }
                else if (Bed2Temperature >= 127)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 2 temperature is elevated", AlertLevel.HighWarning);
                }
                else if (Bed2Temperature <= TemperatureLowerLimit)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 2 temperature is below minimum", AlertLevel.LowError);
                }
                else if (Bed2Temperature <= 123)
                {
                    yield return new Alert(nameof(CarbonDioxideRemediation), "Bed 2 temperature is too low", AlertLevel.LowWarning);
                }
                else
                {
                    yield return Alert.Safe(nameof(CarbonDioxideRemediation));
                }
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckRegenerationHeaterTemp();
        }
    }
}
