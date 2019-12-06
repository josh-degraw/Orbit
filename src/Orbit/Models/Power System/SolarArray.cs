using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;

namespace Orbit.Models
{
    public class SolarArray : IAlertableModel
    {
        /// <summary>
        /// rotation of solar panel is hadrware limited to (-215, 215) degrees, and software limited to (-205, 205)
        /// </summary>
        [Range(-215, 215)]
        public int SolarArrayRotation { get; set; }

        [NotMapped]
        public int solarArrayRotationUpperLimit = 215;
        [NotMapped]
        public int solarArrayRotationLowerLimit = -215;
        [NotMapped]
        public int solarArrayRotationTolerance = 10;

        /// <summary>
        /// voltage output from solar arrays whle in sun
        /// </summary>
        [Range(138, 173)]
        public double SolarArrayVoltage { get; set; }

        [NotMapped]
        public double solarArrayVoltageUpperLimit = 178;
        [NotMapped]
        public double solarArrayVoltageLowerLimit = 133;
        [NotMapped]
        public double solarArrayVoltageTolerance = 5;

        //In Kilowatts      
        public double Power { get; set; }

        /// <summary>
        /// True f array is extened, false if not
        /// </summary>
        public bool Deployed { get; set; }

        [NotMapped]
        public string ComponentName => "SolarArray";

        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        private IEnumerable<Alert> CheckSolarArrayRotation()
        {
            if (SolarArrayRotation > solarArrayRotationUpperLimit)
            {
                yield return new Alert(nameof(SolarArrayRotation), "Solar Array Rotation has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (SolarArrayRotation >= (solarArrayRotationUpperLimit - solarArrayRotationTolerance))
            {
                yield return new Alert(nameof(SolarArrayRotation), "Solar Array Rotation has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else if (SolarArrayRotation < solarArrayRotationUpperLimit)
            {
                yield return new Alert(nameof(SolarArrayRotation), "Solar Array Rotation has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (SolarArrayRotation <= (solarArrayRotationUpperLimit + solarArrayRotationTolerance))
            {
                yield return new Alert(nameof(SolarArrayRotation), "Solar Array Rotation has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(SolarArrayRotation));
            }
        }

        private IEnumerable<Alert> CheckSolarArrayVoltage()
        {
            if (SolarArrayVoltage > solarArrayVoltageUpperLimit)
            {
                yield return new Alert(nameof(SolarArrayVoltage), "Solar array voltage has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (SolarArrayVoltage >= (solarArrayVoltageUpperLimit - solarArrayVoltageTolerance))
            {
                yield return new Alert(nameof(SolarArrayVoltage), "Solar array voltage has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else if (SolarArrayVoltage < solarArrayVoltageUpperLimit)
            {
                yield return new Alert(nameof(SolarArrayVoltage), "Solar array voltage has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (SolarArrayVoltage <= (solarArrayVoltageUpperLimit + solarArrayVoltageTolerance))
            {
                yield return new Alert(nameof(SolarArrayVoltage), "Solar array voltage has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(SolarArrayRotation));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            yield break;
        }
    }
}