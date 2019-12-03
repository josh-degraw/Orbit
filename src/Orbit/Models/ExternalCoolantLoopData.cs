using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Orbit.Models
{
    public class ExternalCoolantLooop : IAlertableModel
    {
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// position in degrees from neutral point. 
        /// </summary>
        [Range(-215, 215)]
        public int RadiatorRotation { get; set; }

        /// <summary>
        /// pressure of fluid in the lines
        /// </summary>
        [Range(0, 600)]
        public int FluidLinePressure { get; set; }

        /// <summary>
        /// temperature of fluid returning from radiator flow control valve to internal/external heat exchanger
        /// </summary>
        [Range(-157, 121)]
        public double TempFluidToHeatExchanger { get; set; }

        /// <summary>
        /// heats the external coolant if the heat offload from the station is too low to warm the -40F degree
        /// ammonia to at least 36F to prevent the internal coolant (water) from freezing
        /// </summary>
        public bool LineHeaterOn { get; set; }

        /// <summary>
        /// contains repleneshment fluid and serves as additional resevoir for expanded fluid to go (increase of pressure),
        /// and draw from when fluid contracts (decrease pressure)
        /// </summary>
        [Range(0, 100)]
        public int TankLevel { get; set; }


        public string ComponentName = "ExternalCoolantSystem";

        [NotMapped]
        int radiatorRotationUpperAlarm = 215;
        [NotMapped]
        int radiatorRotationLowerAlarm = -215;
        [NotMapped]
        int radiatorRotationTolerance = 9;
        private IEnumerable<Alert> CheckRadiatorRotation()
        {
            if(RadiatorRotation > radiatorRotationUpperAlarm)
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if(RadiatorRotation >= (radiatorRotationUpperAlarm - radiatorRotationTolerance))
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else if(RadiatorRotation < radiatorRotationLowerAlarm)
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if(RadiatorRotation <= (radiatorRotationLowerAlarm + radiatorRotationTolerance))    
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else
            {
                yield return new Alert.Safe(nameof(RadiatorRotation));
            }
        }

        [NotMapped]
        int fluidLinePressureUpperAlarm = 480;
        [NotMapped]
        int fluidLinePressureLowerAlarm = 170;
        [NotMapped]
        int fluidLinePressureTolerance = 30;
        private IEnumerable<Alert> CheckLinePressure()
        {
            if (FluidLinePressure >= fluidLinePressureUpperAlarm)
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is above maximum", AlertLevel.HighError);
                PumpOn = false;

            }
            else if (FluidLinePressure >= (fluidLinePressureUpperAlarm - fluidLinePressureTolerance))
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is too high", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else if (FluidLinePressure < fluidLinePressureLowerAlarm)
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is below minimum", AlertLevel.HighError);
                PumpOn = false;
            }
            else if (FluidLinePressure <= (fluidLinePressureLowerAlarm + fluidLinePressureTolerance))
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is too low", AlertLevel.HighWarning);
                PumpOn = true;
            }
            else
            {
                yield return new Alert.Safe(nameof(FluidLinePressure));
                PumpOn = true;
            }
        }

        [NotMapped]
        double tempFluidToHeatExchangerUpperAlarm = 18.22;
        [NotMapped]
        double tempFluidToHeatExchangerLowerAlarm = 1.67;
        [NotMapped]
        double tempFluidToHeatExchangerTolerance = 5;
        private IEnumerable<Alert> CheckFluidTemp()
        {
            if(TempFluidToHeatExchanger >= tempFluidToHeatExchangerUpperAlarm)
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is above maximum", AlertLevel.HighWarning);
                LineHeaterOn = false;
            }
            else if(TempFluidToHeatExchanger >= (tempFluidToHeatExchangerUpperAlarm - tempFluidToHeatExchangerTolerance))
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is too high", AlertLevel.HighError);
                LineHeaterOn = false;
            }
            else if (TempFluidToHeatExchanger <= tempFluidToHeatExchangerLowerAlarm)
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is below minimum", AlertLevel.HighWarning);
                LineHeaterOn = false;
            }
            else if (TempFluidToHeatExchanger <= 2.2)
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is low", AlertLevel.HighError);
                LineHeaterOn = true;
            }
            else
            {
                yield return new Alert.Safe(nameof(TempFluidToHeatExchanger));
                LineHeaterOn = false;
            }

        }

        int tankLevelUpperAlarm = 95;
        int tankLevelTolerance = 5;
        private IEnumerable<Alert> CheckTankLevel()
        {
            if (TankLevel >= 100)
            {
                yield return new Alert(nameof(TankLevel), "Tank level at capacity", AlertLevel.HighError);
            }
            else if (TankLevel >= 95)
            {
                yield return new Alert(nameof(TankLevel), "Tank level nearing capacity", AlertLevel.HighWarning);
            }
            else
            {
                yield return new Alert.Safe(nameof(TankLevel));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckRadiatorRotation().Concat(CheckLinePressure()).Concat(CheckFluidTemp()).Concat(CheckTankLevel());
        }

    }
}                                                                      
