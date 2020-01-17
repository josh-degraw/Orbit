using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class Atmosphere : IAlertableModel
    {
        #region Limits
        //values in decibels
        private const int cabinAmbientNoiseTolerance = 5;
        private const int cabinAmbientNoiseUpperLimit = 72;

        // values in ppm
        private const double CarbonDioxideLowerLimit = 0;
        private const double cabinCarbonDioxideUpperLimit = 4;
        private const double cabinCarbonDioxideTolerance = 2;

        // values are percentages
        private const double cabinHumidityLevelLowerLimit = 30;
        private const double cabinHumidityLevelTolerance = 10;
        private const double cabinHumidityLevelUpperLimit = 80;

        // values are percentages
        private const double cabinOxygenLevelLowerLimit = 17;
        private const double cabinOxygenLevelTolerance = 3;
        private const double cabinOxygenLevelUpperLimit = 25.9;

        // values in psia
        private const double cabinPressureLowerLimit = 50;
        private const double cabinPressureTolerance = 5;
        private const double cabinPressureUpperLimit = 110;

        // degrees Celsius
        private const int cabinTemperatureCrewedLowerLimit = 17;
        private const int cabinTemperatureTolerance = 3;

        private const int cabinTemperatureUncrewedLowerLimit = 4;
        private const int cabinTemperatureUpperLimit = 30;

        // values are percentage
        private const int fanSpeedLowerLimit = 20;
        private const int fanSpeedTolerance = 10;
        private const int fanSpeedUpperLimit = 100;

        #endregion Limits

        #region Public Properties

        [NotMapped]
        public string ComponentName => "Atmosphere";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Decibel value of cabin noise
        /// </summary>
        [Range(0, 90)]
        public double CabinAmbientNoiseLevel { get; set; }

        /// <summary> Goal is &lt; 267 Pa/mm hg (2600ppm) per 24hr period as a maximum </summary>
        public double CabinCarbonDioxideLevel { get; set; }

        /// <summary>
        /// Crewed: 40-75%; uncrewed: 30-80% (is allowed for up to 24hrs while crewed)
        /// </summary>
        [Range(0, 100)]
        public double CabinHumidityLevel { get; set; }

        /// <summary>
        /// desired humidity level set by crew
        /// </summary>
        public double HumiditySetLevel { get; set; }

        /// <summary>
        /// link to Oxygen Generation System values used here are concentrations as a percentage for operational
        /// pressure of 101kPa (14.7psia)
        /// </summary>
        [Range(0, 100)]
        public double CabinOxygenLevel { get; set; }

        /// <summary>
        /// Pressure used are in kPa and is what is listed as nominal for low EVA modules per interoperability standards
        /// Nominal is 101kpa
        /// </summary>
        [Range(0, 120)]
        public double CabinPressure { get; set; }

        /// <summary>
        /// general status of life support and environment as a whole
        /// </summary>
        public SystemStatus CabinStatus { get; set; }

        /// <summary>
        /// status of the temperature and humidity control system
        /// </summary>
        public SystemStatus AtmosphereSystemStatus { get; set; }

        /// <summary>
        /// Ambient air temperature Nominal range is 20-27C uncrewed min is 4C
        /// </summary>
        [Range(-10, 100)]
        public double CabinTemperature { get; set; }

        /// <summary>
        /// desired ambient temperature set by crew
        /// </summary>
        public double SetTemperature { get; set; }

        /// <summary>
        /// position of the heat exchanger air baffles: 
        ///     0 =  all air bypasses heat exhanger 
        ///     100 = all air passes through heat exchanger
        /// </summary>
        public int TempControlValvePosition { get; set; }

        /// <summary>
        /// air/condensate seperator
        /// </summary>
        public bool SeperatorOn { get; set; }
        
        /// <summary>
        /// Air circulation fan speed
        /// </summary>
        [Range(0, 100)]
        public int FanSpeed { get; set; }

        /// <summary>
        /// temperature of the air leaving the heat exchanger to cabin
        /// </summary>
        public double OutflowAirTemperature { get; set; }
        /// <summary>
        /// sensor that will trigger if liquid is detected in air leaving the heat exchanger
        /// </summary>
        public bool LiquidInOutflow { get; set; }

        /// <summary>
        /// redirects outflow air back to heat exchange/condensor if liquid is detected
        /// </summary>
        public DiverterValvePositions ReprocessBafflePosition { get; set; }

        /// <summary>
        /// denotes if station is occupied (false) or not (true)
        /// TODO: move this to a station controller class
        /// </summary>
        public bool UncrewedModeOn { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void ProcessData()
        {
            if(AtmosphereSystemStatus == SystemStatus.Processing)
            {
                if((TempControlValvePosition == 0) || (CabinTemperature < SetTemperature) || (CabinHumidityLevel < HumiditySetLevel))
                {
                    Standby();
                    return;
                }

                if(SeperatorOn == false || FanSpeed <= 0 || ((CabinHumidityLevel > HumiditySetLevel) && (TempControlValvePosition == 100)) )
                {
                    AtmosphereSystemStatus = SystemStatus.Trouble;
                }
                
                if (LiquidInOutflow)
                {
                    ReprocessBafflePosition = DiverterValvePositions.Reprocess;
                }
                else
                {
                    ReprocessBafflePosition = DiverterValvePositions.Accept;
                }

                if ((CabinHumidityLevel > HumiditySetLevel) || (CabinTemperature > SetTemperature))
                {
                    TempControlValvePosition++;
                }
                else if ((CabinHumidityLevel < HumiditySetLevel) || (CabinTemperature < SetTemperature))
                {
                    TempControlValvePosition--;
                }
            } 
            else if(AtmosphereSystemStatus == SystemStatus.Standby)
            {
                if((CabinHumidityLevel > HumiditySetLevel) || (CabinTemperature > SetTemperature))
                {
                    Process();
                    return;
                }

                if(SeperatorOn || FanSpeed > 0)
                {
                    AtmosphereSystemStatus = SystemStatus.Trouble;
                }
            }
        }

        /// <summary>
        /// change system to a processing or 'active' state
        /// </summary>
        private void Process()
        {
            AtmosphereSystemStatus = SystemStatus.Processing;
            SeperatorOn = true;
            FanSpeed = 50;
        }

        /// <summary>
        /// change system to a standby or 'off' state
        /// </summary>
        private void Standby()
        {
            AtmosphereSystemStatus = SystemStatus.Standby;
            SeperatorOn = false;
            FanSpeed = 0;
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckCabinPressure()
                .Concat(this.CheckCabinOxygenLevel())
                .Concat(this.CheckCabinCarbonDioxideLevel())
                .Concat(this.CheckCabinHumidityLevel())
                .Concat(this.CheckCabinAmbientNoiseLevel())
                .Concat(this.CheckCabinTemperature())
                .Concat(this.CheckFanSpeed())
                .Concat(this.CheckSeperator())
                .Concat(this.CheckFanOn());
        }

        #endregion Public Methods

        #region Check Alerts

        private IEnumerable<Alert> CheckCabinAmbientNoiseLevel()
        {
            if (CabinAmbientNoiseLevel > cabinAmbientNoiseUpperLimit)
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

        private IEnumerable<Alert> CheckCabinCarbonDioxideLevel()
        {
            if (CabinCarbonDioxideLevel >= cabinCarbonDioxideUpperLimit)
            {
                yield return new Alert(nameof(CabinCarbonDioxideLevel), "Carbon dioxide level is above maximum", AlertLevel.HighError);
            }
            else if (CabinCarbonDioxideLevel >= (cabinCarbonDioxideUpperLimit - cabinCarbonDioxideTolerance))
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
            if (CabinHumidityLevel >= cabinHumidityLevelUpperLimit)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is above maximum", AlertLevel.HighError);
            }
            else if (CabinHumidityLevel >= (cabinHumidityLevelUpperLimit - cabinHumidityLevelTolerance))
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is elevated", AlertLevel.HighWarning);
            }
            else if (CabinHumidityLevel <= cabinHumidityLevelLowerLimit)
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is below minimum", AlertLevel.LowError);
            }
            else if (CabinHumidityLevel <= (cabinHumidityLevelLowerLimit + cabinHumidityLevelTolerance))
            {
                yield return new Alert(nameof(CabinHumidityLevel), "Cabin humidity is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinHumidityLevel));
            }
        }

        private IEnumerable<Alert> CheckCabinOxygenLevel()
        {
            if (CabinOxygenLevel >= cabinOxygenLevelUpperLimit)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is above maximum", AlertLevel.HighError);
            }
            else if (CabinOxygenLevel >= (cabinOxygenLevelUpperLimit - cabinOxygenLevelTolerance))
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is elevated", AlertLevel.HighWarning);
            }
            else if (CabinOxygenLevel <= cabinOxygenLevelLowerLimit)
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration below minimum", AlertLevel.LowError);
            }
            else if (CabinOxygenLevel <= (cabinOxygenLevelLowerLimit + cabinOxygenLevelTolerance))
            {
                yield return new Alert(nameof(CheckCabinOxygenLevel), "Cabin oxygen concentration is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinOxygenLevel));
            }
        }

        private IEnumerable<Alert> CheckCabinPressure()
        {
            if (CabinPressure >= cabinPressureUpperLimit)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure has exceeded maximum", AlertLevel.HighError);
            }
            else if (CabinPressure >= (cabinPressureUpperLimit - cabinPressureTolerance))
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure is elevated", AlertLevel.HighWarning);
            }
            else if (CabinPressure <= cabinPressureLowerLimit)
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure below minimum", AlertLevel.LowError);
            }
            else if (CabinPressure < (cabinPressureLowerLimit + cabinPressureTolerance))
            {
                yield return new Alert(nameof(CabinPressure), "Cabin pressure is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinPressure));
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
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is below minimum for uncrewed mode", AlertLevel.LowError);
            }
            else if (!UncrewedModeOn && (CabinTemperature < cabinTemperatureCrewedLowerLimit))
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is below minimum", AlertLevel.LowError);
            }
            else if (!UncrewedModeOn && (CabinTemperature <= (cabinTemperatureCrewedLowerLimit - cabinTemperatureTolerance)))
            {
                yield return new Alert(nameof(CabinTemperature), "Cabin temperature is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(CabinTemperature));
            }
        }

        private IEnumerable<Alert> CheckFanSpeed()
        {
            if (FanSpeed > fanSpeedUpperLimit)
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is above maximum", AlertLevel.HighError);
            }
            else if (FanSpeed >= (fanSpeedUpperLimit - fanSpeedTolerance))
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is high", AlertLevel.HighWarning);
            }
            else if (FanSpeed < fanSpeedLowerLimit)
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is below minimum", AlertLevel.LowError);
            }
            else if (FanSpeed <= (fanSpeedLowerLimit - fanSpeedTolerance))
            {
                yield return new Alert(nameof(FanSpeed), "Fan speed is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(FanSpeed));
            }
        }

        private IEnumerable<Alert> CheckSeperator()
        {
            if (AtmosphereSystemStatus == SystemStatus.Processing)
            {
                if (!SeperatorOn)
                {
                    yield return new Alert(nameof(Atmosphere), "Seperator is not on while processing", AlertLevel.HighError);
                }
                else
                {
                    yield return Alert.Safe(nameof(Atmosphere));
                }
            }
            else if (AtmosphereSystemStatus == SystemStatus.Standby)
            {
                if (SeperatorOn)
                {
                    yield return new Alert(nameof(Atmosphere), "Seperator is on while in standby", AlertLevel.HighError);
                }
                else
                {
                    yield return Alert.Safe(nameof(Atmosphere));
                }
            }
        }

        private IEnumerable<Alert> CheckFanOn()
        {
            if (AtmosphereSystemStatus == SystemStatus.Processing)
            {
                if (FanSpeed <= 0)
                {
                    yield return new Alert(nameof(Atmosphere), "Fan off while processing", AlertLevel.LowError);
                }
                else
                {
                    yield return Alert.Safe(nameof(Atmosphere));
                }
            }
            else if (AtmosphereSystemStatus == SystemStatus.Standby)
            {
                if (FanSpeed > 0)
                {
                    yield return new Alert(nameof(Atmosphere), "Fan on while in standby", AlertLevel.HighWarning);
                }
            }
        }

        #endregion Check Alerts

    }
}