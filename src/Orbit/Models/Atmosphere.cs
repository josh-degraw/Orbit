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
        public SystemStatus CabinStatus { get; set; }

        /// <summary>
        /// denotes if station is occupied (false) or not (true)
        /// </summary>
        public bool UncrewedModeOn { get; set; }

        /// <summary>
        /// Pressure used are in kPa and is what is listed as nominal for low EVA modules per interoperability standards
        /// Nominal is 101kpa
        /// </summary>
        [Range(0, 120)]
        public double CabinPressure { get; set; }

        [NotMapped]
        public double cabinPressureUpperLimit = 110;
        [NotMapped]
        public double cabinPressureLowerLimit = 50;
        [NotMapped]
        public double cabinPressureTolerance = 5;

        /// <summary>
        /// link to Oxygen Generation System
        /// values used here are concentrations as a percentage for operational pressure of 101kPa (14.7psia)
        /// </summary>
        [Range(0, 100)]
        public double CabinOxygenLevel { get; set; }

        [NotMapped]
        public double cabinOxygenLevelUpperLimit = 25.9;
        [NotMapped]
        public double cabinOxygenLevelLowerLimit = 17;
        [NotMapped]
        public double cabinOxygenLevelTolerance = 3;

        /// <summary>
        /// Goal is < 267 Pa/mm hg (2600ppm) per 24hr period as a maximum  
        /// </summary>
        public double CabinCarbonDioxideLevel { get; set; }

        [NotMapped]
        public double cabinCarbonDioxideUpperLimit = 2600;
        [NotMapped]
        public double cabinCarbonDioxideTolerance = 600;

        /// <summary>
        /// Air circulation fan speed
        /// </summary>
        [Range(0, 100)]
        public int FanSpeed { get; set; }

        [NotMapped]
        public int fanSpeedUpperLimit = 100;
        [NotMapped]
        public int fanSpeedLowerLimit = 20;
        [NotMapped]
        public int fanSpeedTolerance = 10;

        /// <summary>
        /// Ambient air temperature Nominal range is 20-27C
        /// uncrewed min is 4C
        /// </summary>
        [Range(-10, 100)]
        public int CabinTemperature { get; set; }

        [NotMapped]
        public int cabinTemperatureUpperLimit = 30;
        [NotMapped]
        public int cabinTemperatureCrewedLowerLimit = 17;
        [NotMapped]
        public int cabinTemperatureUncrewedLowerLimit = 4;
        [NotMapped]
        public int cabinTemperatureTolerance = 3;


        /// <summary>
        /// Crewed: 40-75%; uncrewed: 30-80% (is allowed for up to 24hrs while crewed)
        /// </summary>
        [Range(0, 100)]
        public double CabinHumidityLevel { get; set; }

        [NotMapped]
        public double cabinHumidityLevelUpperLimit = 80;
        [NotMapped]
        public double cabinHumidityLevelLowerLimit = 30;
        [NotMapped]
        public double cabinHumidityLevelTolerance = 10;

        /// <summary>
        /// decibal value of cabin noise
        /// </summary>
        [Range(0, 90)]
        public double CabinAmbientNoiseLevel { get; set; }

        [NotMapped]
        public int cabinAmbientNoiseUpperLimit = 72;
        [NotMapped]
        public int cabinAmbientNoiseTolerance = 5;

        #region CheckLevelMethods
        private IEnumerable<Alert> CheckCabinPressure()
        {
            if(CabinPressure >= cabinPressureUpperLimit)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure has exceeded maximum", AlertLevel.HighError);
            }
            else if(CabinPressure >= (cabinPressureUpperLimit - cabinPressureTolerance))
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure is elevated", AlertLevel.HighWarning);
            }
            else if(CabinPressure <= (cabinPressureLowerLimit + cabinPressureTolerance))
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure has decreased", AlertLevel.LowError);
            }
            else if(CabinPressure < cabinPressureLowerLimit)
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
            if(CabinOxygenLevel >= cabinOxygenLevelUpperLimit)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is above maximum", AlertLevel.HighError);
            }
            else if(CabinOxygenLevel >= (cabinOxygenLevelUpperLimit - cabinOxygenLevelTolerance))
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is elevated", AlertLevel.HighWarning);
            }
            else if(CabinOxygenLevel <= (cabinOxygenLevelLowerLimit + cabinOxygenLevelTolerance))
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is decreased", AlertLevel.HighError);
            }
            else if(CabinOxygenLevel <= cabinOxygenLevelLowerLimit)
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
            if(CabinCarbonDioxideLevel >= cabinCarbonDioxideUpperLimit)
            {
                yield return new Alert(nameof(CabinCarbonDioxideLevel), "Carbon dioxide level is above maximum", AlertLevel.HighError);
            }
            else if(CabinCarbonDioxideLevel >= (cabinCarbonDioxideUpperLimit - cabinCarbonDioxideTolerance))
            {
                yield return new Alert(nameof(CabinCarbonDioxideLevel), "Cabin carbon dioxide is nearing maximum", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinCarbonDioxideLevel));
            }
        }

        private IEnumerable<Alert> CheckFanSpeed()
        {
            if(FanSpeed > fanSpeedUpperLimit)
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is above maximum", AlertLevel.HighError);
            }
            else if ( FanSpeed >= (fanSpeedUpperLimit - fanSpeedTolerance))
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is high", AlertLevel.HighWarning);
            }
            else if(FanSpeed < fanSpeedLowerLimit)
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is below minimum", AlertLevel.LowError);
            }
            else if(FanSpeed <= (fanSpeedLowerLimit - fanSpeedTolerance))
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(FanSpeed));
            }
        }

        private IEnumerable<Alert> CheckCabinTemperature()
        {
            if (CabinTemperature > cabinTemperatureUpperLimit)
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is above maximum", AlertLevel.HighError);
            }
            else if (CabinTemperature >= (cabinTemperatureUpperLimit - cabinTemperatureTolerance))
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is high", AlertLevel.HighWarning);
            }
            else if (UncrewedModeOn && (CabinTemperature <= cabinTemperatureUncrewedLowerLimit))
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is below minimum for uncrewed mode", AlertLevel.HighError);
            }
            else if (CabinTemperature < cabinTemperatureCrewedLowerLimit)
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is below minimum", AlertLevel.LowError);
            }
            else if (CabinTemperature <= (cabinTemperatureCrewedLowerLimit - cabinTemperatureTolerance))
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinTemperature));
            }
        }


        private IEnumerable<Alert> CheckCabinHumidityLevel()
        {
            if(CabinHumidityLevel >= cabinHumidityLevelUpperLimit)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is above maximum", AlertLevel.HighError);
            }
            else if(CabinHumidityLevel >= (cabinHumidityLevelUpperLimit - cabinHumidityLevelTolerance))
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is elevated", AlertLevel.HighWarning);
            }
            else if(CabinHumidityLevel <= (cabinHumidityLevelLowerLimit + cabinHumidityLevelTolerance))
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is low", AlertLevel.LowWarning);
            }
            else if (CabinHumidityLevel <= cabinHumidityLevelLowerLimit)
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
            if(CabinAmbientNoiseLevel > cabinAmbientNoiseUpperLimit)
            {
                yield return new Alert(nameof(CabinAmbientNoiseLevel), "Cabin noise has exceeded maximum", AlertLevel.HighError);
            }
            else if (CabinAmbientNoiseLevel >= (cabinAmbientNoiseUpperLimit - cabinAmbientNoiseTolerance))
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
