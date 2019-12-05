using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Orbit.Models
{
    public class Atmosphere: IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "Atmosphere";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;
       
        /// <summary>
        /// general status of lifesupport and environment as a whole
        /// </summary>
        public string CabinStatus { get; set; }

        /// <summary>
        /// Pressure used are in kPa and is what is listed as nominal for low EVA modules per interoperability standards
        /// </summary>
        [Range(0, 120)]
        public double CabinPressure { get; set; }

        [NotMapped]
        public double CabinPressureUpperAlarm = 110;
        [NotMapped]
        public double CabinPressureLowerAlarm = 50;
        [NotMapped]
        public double CabinPressureNominal = 101;

        /// <summary>
        /// link to Oxygen Generation System
        /// values used here are concentrations as a percentage for operational pressure of 101kPa (14.7psia)
        /// </summary>
        [Range(0, 100)]
        public double CabinOxygenLevel { get; set; }

        [NotMapped]
        public double CabinOxygenLevelUpperAlarm = 25.9;
        [NotMapped]
        public double CabinOxygenLevelLowerAlarm = 17;

        /// <summary>
        /// Goal is < 267 Pa/mm hg (2600ppm) per 24hr period as a maximum  
        /// </summary>
        public double CabinCarbonDioxideLevel { get; set; }

        [NotMapped]
        public double CabinCarbonDioxideUpperAlarm = 2600;

        /// <summary>
        /// Crewed: 40-75%; uncrewed: 30-80% (is allowed for up to 24hrs while crewed)
        /// </summary>
        [Range(0, 100)]
        public double CabinHumidityLevel { get; set; }

        [NotMapped]
        public double CabinHumidityLevelUpperAlarm = 80;
        [NotMapped]
        public double CabinHumidityLevelLowerAlarm = 30;

        /// <summary>
        /// decibal value of cabin noise
        /// </summary>
        [Range(0, 90)]
        public double CabinAmbientNoiseLevel { get; set; }

        [NotMapped]
        public int CabinAmbientNoiseUpperAlarm = 72;

        #region CheckLevelMethods
        private IEnumerable<Alert> CheckCabinPressure()
        {
            if(CabinPressure >= CabinPressureUpperAlarm)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure has exceeded maximum", AlertLevel.HighError);
            }
            else if(CabinPressure >= 105)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure is elevated", AlertLevel.HighWarning);
            }
            else if(CabinPressure <= 55)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure has decreased", AlertLevel.LowError);
            }
            else if(CabinPressure < CabinPressureLowerAlarm)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure is below minimum", AlertLevel.LowError);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinPressure));
            }
        }

        private IEnumerable<Alert> CheckCabinOxygenLevel()
        {
            if(CabinOxygenLevel >= CabinOxygenLevelUpperAlarm)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is above maximum", AlertLevel.HighError);
            }
            else if(CabinOxygenLevel >= 24)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is elevated", AlertLevel.HighWarning);
            }
            else if(CabinOxygenLevel <= 18)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is decreased", AlertLevel.HighError);
            }
            else if(CabinOxygenLevel <= CabinOxygenLevelLowerAlarm)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is below minimum", AlertLevel.LowError);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinOxygenLevel));
            }
        }

        private IEnumerable<Alert> CheckCabinCarbonDioxideLevel()
        {
            if(CabinCarbonDioxideLevel >= 2600)
            {
                yield return new Alert(nameof(CabinCarbonDioxideLevel), "Carbon dioxide level is above maximum", AlertLevel.HighError);
            }
            else if(CabinCarbonDioxideLevel >= 2000)
            {
                yield return new Alert(nameof(CabinCarbonDioxideLevel), "Cabin carbon dioxide is nearing maximum", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinCarbonDioxideLevel));
            }
        }

        private IEnumerable<Alert> CheckCabinHumidityLevel()
        {
            if(CabinHumidityLevel >= CabinHumidityLevelUpperAlarm)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is above maximum", AlertLevel.HighError);
            }
            else if(CabinHumidityLevel >= 70)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is elevated", AlertLevel.HighWarning);
            }
            else if(CabinHumidityLevel <= 40)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is low", AlertLevel.LowWarning);
            }
            else if (CabinHumidityLevel <= 30)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is below minimum", AlertLevel.LowError);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinHumidityLevel));
            }
        }

        private IEnumerable<Alert> CheckCabinAmbientNoiseLevel()
        {
            if(CabinAmbientNoiseLevel > CabinAmbientNoiseUpperAlarm)
            {
                yield return new Alert(nameof(CabinAmbientNoiseLevel), "Cabin noise has exceeded maximum", AlertLevel.HighError);
            }
            else if (CabinAmbientNoiseLevel >= 68)
            {
                yield return new Alert(nameof(CabinAmbientNoiseLevel), "Cabin noise is elevated", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinAmbientNoiseLevel));
            }
        }
        #endregion CheckLevelMethods

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckCabinPressure().Concat(CheckCabinOxygenLevel()).Concat(CheckCabinCarbonDioxideLevel()).Concat(CheckCabinHumidityLevel()).Concat(CheckCabinAmbientNoiseLevel());
        }
    }
}
