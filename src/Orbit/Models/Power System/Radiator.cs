using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Orbit.Models
{
    public class Radiator: IAlertableModel
    {
        /// This class is essentially a copy of an ExternalThermalSystem class. 
        /// Is there a better way to set up a table in entity without copying everything?


        /// <summary>
        /// Pressure of the ammonia in the battery cooling system lines
        /// </summary>
        [Range(0, 600)]
        public int FluidPressure { get; set; }

        [NotMapped]
        public int fluidPressureUpperLimit = 480;
        [NotMapped]
        public int fluidPressureLowerLimit = 170;
        [NotMapped]
        public int fluidPressureTolerance = 30;

        /// <summary>
        /// Temperature of the ammonia within the battery cooling system going to the battery heat exchanger
        /// </summary>
        [Range(-157, 215)]
        public int FluidTemperature { get; set; }

        [NotMapped]
        public double FluidTemperatureUpperLimit = 18.22;
        [NotMapped]
        public double fluidTemperatureLowerLimit = 1.67;
        [NotMapped]
        public double fluidTemperatureTolerance = 5;
        
        /// <summary>
        /// note if radiator is extended (deployed) or retracted
        /// </summary>
        public bool Deployed { get; set; }

        /// <summary>
        /// position of radiator from neutral in degrees as it moderates the fluid temperature
        /// </summary>
        [Range(-215, 215)]
        public int Rotation { get; set; }

        [NotMapped]
        int radiatorRotationUpperLimit = 215;
        [NotMapped]
        int radiatorRotationLowerLimit = -215; 
        [NotMapped]
        int radiatorRotationTolerance = 9;

        /// <summary>
        /// Fluid circulation pump for battery cooling system
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// contains repleneshment fluid and serves as additional resevoir for expanded fluid to go (increase of pressure),
        /// and draw from when fluid contracts (decrease pressure)
        /// </summary>
        [Range(0, 100)]
        public int TankLevel { get; set; }
        [NotMapped]
        int tankLevelUpperLimit = 95;
        [NotMapped]
        int tankLevelTolerance = 5;

        [NotMapped]
        public string ComponentName => "BatteryRadiator";
        
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        private IEnumerable<Alert> CheckRadiatorRotation()
        {
            if (Rotation > radiatorRotationUpperLimit)
            {
                yield return new Alert(nameof(Rotation), "Coolant radiator has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (Rotation >= (radiatorRotationUpperLimit - radiatorRotationTolerance))
            {
                yield return new Alert(nameof(Rotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else if (Rotation < radiatorRotationLowerLimit)
            {
                yield return new Alert(nameof(Rotation), "Coolant radiator has exceeded minimum available rotation", AlertLevel.HighError);
            }
            else if (Rotation <= (radiatorRotationLowerLimit + radiatorRotationTolerance))
            {
                yield return new Alert(nameof(Rotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(Rotation));
            }
        }

        private IEnumerable<Alert> CheckLinePressure()
        {
            if (FluidPressure >= fluidPressureUpperLimit)
            {
                PumpOn = false;
                yield return new Alert(nameof(FluidPressure), "Coolant line pressure is above maximum", AlertLevel.HighError);

            }
            else if (FluidPressure >= (fluidPressureUpperLimit - fluidPressureTolerance))
            {
                PumpOn = true;
                yield return new Alert(nameof(FluidPressure), "Coolant line pressure is too high", AlertLevel.HighWarning);
            }
            else if (FluidPressure < fluidPressureLowerLimit)
            {
                PumpOn = false;
                yield return new Alert(nameof(FluidPressure), "Coolant line pressure is below minimum", AlertLevel.HighError);
            }
            else if (FluidPressure <= (fluidPressureLowerLimit + fluidPressureTolerance))
            {
                PumpOn = true;
                yield return new Alert(nameof(FluidPressure), "Coolant line pressure is too low", AlertLevel.HighWarning);
            }
            else
            {
                PumpOn = true;
                yield return Alert.Safe(nameof(FluidPressure));
            }
        }

        private IEnumerable<Alert> CheckFluidTemp()
        {
            if (FluidTemperature >= FluidTemperatureUpperLimit)
            {
                yield return new Alert(nameof(FluidTemperature), "External coolant temperature is above maximum", AlertLevel.HighWarning);
            }
            else if (FluidTemperature >= (fluidPressureUpperLimit - fluidTemperatureTolerance))
            {
                yield return new Alert(nameof(FluidTemperatureUpperLimit), "External coolant temperature is too high", AlertLevel.HighError);
            }
            else if (FluidTemperature <= fluidTemperatureLowerLimit)
            {
                yield return new Alert(nameof(FluidTemperature), "External coolant temperature is below minimum", AlertLevel.HighWarning);
            }
            else if (FluidTemperature <= 2.2)
            {
                yield return new Alert(nameof(FluidTemperature), "External coolant temperature is low", AlertLevel.HighError);
            }
            else
            {
                yield return Alert.Safe(nameof(FluidTemperature));
            }
        }

        private IEnumerable<Alert> CheckTankLevel()
        {
            if (TankLevel >= tankLevelUpperLimit)
            {
                yield return new Alert(nameof(TankLevel), "Tank level at capacity", AlertLevel.HighError);
            }
            else if (TankLevel >= (tankLevelUpperLimit - tankLevelTolerance))
            {
                yield return new Alert(nameof(TankLevel), "Tank level nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(TankLevel));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckRadiatorRotation().Concat(CheckLinePressure()).Concat(CheckFluidTemp()).Concat(CheckTankLevel()).Concat(CheckTankLevel()); ;
        }
    }
}
