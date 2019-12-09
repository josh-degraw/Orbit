using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Orbit.Models
{
    public class ExternalCoolantLoopData : IAlertableModel
    {
        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// basically on/off/failure(on by not working)
        /// </summary>
        public bool PumpOn { get; set; }

        /// <summary>
        /// position in degrees from neutral point.
        /// </summary>
        [Range(-215, 215)]
        public int RadiatorRotation { get; set; }

        private const int radiatorRotationUpperLimit = 215;
        private const int radiatorRotationLowerLimit = -215;
        private const int radiatorRotationTolerance = 10;

        /// <summary>
        /// pressure of fluid in the lines
        /// </summary>
        [Range(0, 600)]
        public int FluidLinePressure { get; set; }

        private const int fluidLinePressureUpperLimit = 480;
        private const int fluidLinePressureLowerLimit = 170;
        private const int fluidLinePressureTolerance = 30;

        /// <summary>
        /// temperature of fluid returning from radiator flow control valve to internal/external heat exchanger
        /// </summary>
        [Range(-157, 121)]
        public double TempFluidToHeatExchanger { get; set; }

        private const double tempFluidToHeatExchangerUpperLimit = 18.22;
        private const double tempFluidToHeatExchangerLowerLimit = 1.67;
        private const double tempFluidToHeatExchangerTolerance = 5;

        /// <summary>
        /// heats the external coolant if the heat offload from the station is too low to warm the -40F degree ammonia
        /// to at least 36F to prevent the internal coolant (water) from freezing
        /// </summary>
        public bool LineHeaterOn { get; set; }

        /// <summary>
        /// contains replenishment fluid and serves as additional reservoir for expanded fluid to go (increase of
        /// pressure), and draw from when fluid contracts (decrease pressure)
        /// </summary>
        [Range(0, 100)]
        public int TankLevel { get; set; }

        private const int tankLevelUpperLimit = 95;
        private const int tankLevelTolerance = 5;

        public string ComponentName => "ExternalCoolantSystem";

        private IEnumerable<Alert> CheckRadiatorRotation()
        {
            if (RadiatorRotation > radiatorRotationUpperLimit)
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded maximum available rotation", AlertLevel.HighError);
            }
            else if (RadiatorRotation >= (radiatorRotationUpperLimit - radiatorRotationTolerance))
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.HighWarning);
            }
            else if (RadiatorRotation < radiatorRotationLowerLimit)
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded maximum available rotation", AlertLevel.LowError);
            }
            else if (RadiatorRotation <= (radiatorRotationLowerLimit + radiatorRotationTolerance))
            {
                yield return new Alert(nameof(RadiatorRotation), "Coolant radiator has exceeded allowed rotation", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(RadiatorRotation));
            }
        }

        private IEnumerable<Alert> CheckLinePressure()
        {
            if (FluidLinePressure >= fluidLinePressureUpperLimit)
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is above maximum", AlertLevel.HighError);
            }
            else if (FluidLinePressure >= (fluidLinePressureUpperLimit - fluidLinePressureTolerance))
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is too high", AlertLevel.HighWarning);
            }
            else if (FluidLinePressure < fluidLinePressureLowerLimit)
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is below minimum", AlertLevel.LowError);
            }
            else if (FluidLinePressure <= (fluidLinePressureLowerLimit + fluidLinePressureTolerance))
            {
                yield return new Alert(nameof(FluidLinePressure), "Coolant line pressure is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(FluidLinePressure));
            }
        }

        private IEnumerable<Alert> CheckFluidTemp()
        {
            if (TempFluidToHeatExchanger >= tempFluidToHeatExchangerUpperLimit)
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is above maximum", AlertLevel.HighError);
            }
            else if (TempFluidToHeatExchanger >= (tempFluidToHeatExchangerUpperLimit - tempFluidToHeatExchangerTolerance))
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is too high", AlertLevel.HighWarning);
            }
            else if (TempFluidToHeatExchanger <= tempFluidToHeatExchangerLowerLimit)
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is below minimum", AlertLevel.LowError);
            }
            else if (TempFluidToHeatExchanger <= (tempFluidToHeatExchangerLowerLimit + tempFluidToHeatExchangerTolerance))
            {
                yield return new Alert(nameof(TempFluidToHeatExchanger), "External coolant temperature is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(TempFluidToHeatExchanger));
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
            return this.CheckRadiatorRotation()
                .Concat(this.CheckLinePressure())
                .Concat(this.CheckFluidTemp())
                .Concat(this.CheckTankLevel());
        }
    }
}