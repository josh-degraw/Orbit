using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Orbit.Models
{
    public class ExternalCoolantLoopData : IAlertableModel
    {
        #region Limits

        private const int radiatorRotationUpperLimit = 215;
        private const int radiatorRotationLowerLimit = -215;
        private const int radiatorRotationTolerance = 10;

        private const int fluidPressureUpperLimit = 480;
        private const int fluidPressureLowerLimit = 170;
        private const int fluidPressureTolerance = 30;

        private const double outputFluidTemperatueUpperLimit = 18.22;
        private const double outputFluidTemperatureLowerLimit = 1.67;
        private const double outputFluidTemperatureTolerance = 5;

        private bool radiatorRotationIncreasing = true;

        #endregion Limits

        #region Public Properties
        public string ComponentName => "ExternalCoolantSystem";

        public DateTimeOffset ReportDateTime { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// overall status of the system
        /// </summary>
        public SystemStatus Status { get; set; }

        /// <summary>
        /// position in degrees from neutral point.
        /// </summary>
        [Range(-215, 215)]
        public int RadiatorRotation { get; set; }

        /// <summary>
        /// pump for fluid circuit A
        /// </summary>
        public bool PumpAOn { get; set; }

        /// <summary>
        /// pump for fluid circuit B
        /// </summary>
        public bool PumpBOn { get; set; }

        /// <summary>
        /// position of the valve that mixes 'hot' fluid returning from heat exchanger and 'cold' fluid 
        /// returning from radiator. Acts like a shower temp valve. 0 = all 'hot', 100 = all 'cold'
        /// </summary>
        public int MixValvePosition { get; set; }

        /// <summary>
        /// pressure of fluid in loop A
        /// </summary>
        [Range(0, 600)]
        public int LineAPressure { get; set; }

        /// <summary>
        /// pressure of fluid in loop B
        /// </summary>
        [Range(0, 600)]
        public int LineBPressure { get; set; }

        /// <summary>
        /// heats the external coolant if the heat offload from the station is too low to warm the -40F degree ammonia
        /// to at least 36F to prevent the internal coolant fluid (water) from freezing
        /// </summary>
        public bool LineHeaterOn { get; set; }

        /// <summary>
        /// True if radiator is extended (deployed), false if retracted (not deployed)
        /// </summary>
        public bool RadiatorDeployed { get; set; }

        /// <summary>
        /// temperature of fluid returning from radiator flow control valve to internal/external heat exchanger
        /// </summary>
        [Range(-157, 121)]
        public double OutputFluidTemperature { get; set; }

        /// <summary>
        /// goal output fluid temperature
        /// </summary>
        public double SetTemperature { get; set; }

        #endregion Public Properties

        #region Methods

        public void ProcessData()
        {
            // need code to change from trouble state to 'ok' state if no trouble conditions exist
            // preferebly without checking conditions twice (ie. line pressure check)
            if (!PumpAOn || !PumpBOn)
            {
                Trouble();
            }
            if ((OutputFluidTemperature > outputFluidTemperatueUpperLimit) || (OutputFluidTemperature > SetTemperature))
            {
                // open the mixing valve and allow more 'cold' fluid in the mix
                IncreaseFluidMix();
            }
            if ((OutputFluidTemperature < outputFluidTemperatureLowerLimit) || (OutputFluidTemperature < SetTemperature))
            {
                // close the mixing valve to keep more 'hot' fluid in the mix
                DecreaseFluidMix();
            }
            if ((LineAPressure > fluidPressureUpperLimit) || (LineAPressure < fluidPressureLowerLimit))
            {
                // problem in the fluid line A, shut off pumpA to prevent pump damage 
                PumpAOn = false;
                Trouble();
            }
            if ((LineBPressure > fluidPressureUpperLimit) || (LineBPressure < fluidPressureLowerLimit))
            {
                // problem in the fluid line B, shut off pumpB to prevent pump damage
                PumpBOn = false;
                Trouble();
            }
            if (RadiatorDeployed)
            {
                RotateRadiator();
            }
        }

        private void Trouble()
        {
            // change the system to a trouble status
            Status = SystemStatus.Trouble;
        }

        private void IncreaseFluidMix()
        {
            // need formula: if valve opened 1 'degree', how much would temp change, use difference
            // in outflow and set temp to determine amount to change valve
            
            if(MixValvePosition == 0)
            {
                // heater is on, turn it off
                LineHeaterOn = false;
            }
            MixValvePosition++;
        }

        private void DecreaseFluidMix()
        {
            // need formula: if valve closed 1 'degree', how much would temp change, use difference
            // in outflow and set temp to determine amount to change valve
            
            MixValvePosition--;
            if (MixValvePosition == 0)
            {
                // heat load from station is too low to keep fluid above min temp, turn heater on
                LineHeaterOn = true;
            }
        }

        private void RotateRadiator()
        {
            if((RadiatorRotation > radiatorRotationUpperLimit) ||(RadiatorRotation < radiatorRotationLowerLimit))
            {
                Trouble();
            }
            
            if(radiatorRotationIncreasing && (RadiatorRotation < radiatorRotationUpperLimit))
            {
                RadiatorRotation++;
            }
            else if(!radiatorRotationIncreasing && (RadiatorRotation > radiatorRotationLowerLimit))
            {
                RadiatorRotation--;
            }
            else
            {
                radiatorRotationIncreasing = !radiatorRotationIncreasing;
            }
        }

    #endregion Methods

    #region Alert generation

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

        private IEnumerable<Alert> CheckLineAPressure()
        {
            if (LineAPressure >= fluidPressureUpperLimit)
            {
                yield return new Alert(nameof(LineAPressure), "Coolant line A pressure is above maximum", AlertLevel.HighError);
            }
            else if (LineAPressure >= (fluidPressureUpperLimit - fluidPressureTolerance))
            {
                yield return new Alert(nameof(LineAPressure), "Coolant line A pressure is too high", AlertLevel.HighWarning);
            }
            else if (LineAPressure < fluidPressureLowerLimit)
            {
                yield return new Alert(nameof(LineAPressure), "Coolant line A pressure is below minimum", AlertLevel.LowError);
            }
            else if (LineAPressure <= (fluidPressureLowerLimit + fluidPressureTolerance))
            {
                yield return new Alert(nameof(LineAPressure), "Coolant line A pressure is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(LineAPressure));
            }
        }
        private IEnumerable<Alert> CheckLineBPressure()
        {
            if (LineBPressure >= fluidPressureUpperLimit)
            {
                yield return new Alert(nameof(LineBPressure), "Coolant line B pressure is above maximum", AlertLevel.HighError);
            }
            else if (LineBPressure >= (fluidPressureUpperLimit - fluidPressureTolerance))
            {
                yield return new Alert(nameof(LineBPressure), "Coolant line B  pressure is too high", AlertLevel.HighWarning);
            }
            else if (LineBPressure < fluidPressureLowerLimit)
            {
                yield return new Alert(nameof(LineBPressure), "Coolant line B pressure is below minimum", AlertLevel.LowError);
            }
            else if (LineBPressure <= (fluidPressureLowerLimit + fluidPressureTolerance))
            {
                yield return new Alert(nameof(LineBPressure), "Coolant line B pressure is too low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(LineBPressure));
            }
        }
        private IEnumerable<Alert> CheckFluidTemp()
        {
            if (OutputFluidTemperature >= outputFluidTemperatueUpperLimit)
            {
                yield return new Alert(nameof(OutputFluidTemperature), "External coolant temperature is above maximum", AlertLevel.HighError);
            }
            else if (OutputFluidTemperature >= (outputFluidTemperatueUpperLimit - outputFluidTemperatureTolerance))
            {
                yield return new Alert(nameof(OutputFluidTemperature), "External coolant temperature is too high", AlertLevel.HighWarning);
            }
            else if (OutputFluidTemperature <= outputFluidTemperatureLowerLimit)
            {
                yield return new Alert(nameof(OutputFluidTemperature), "External coolant temperature is below minimum", AlertLevel.LowError);
            }
            else if (OutputFluidTemperature <= (outputFluidTemperatureLowerLimit + outputFluidTemperatureTolerance))
            {
                yield return new Alert(nameof(OutputFluidTemperature), "External coolant temperature is low", AlertLevel.LowWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(OutputFluidTemperature));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return this.CheckRadiatorRotation()
                .Concat(this.CheckLineAPressure())
                .Concat(this.CheckLineBPressure())
                .Concat(this.CheckFluidTemp())                                                ;
        }

        #endregion Alert generation

        #region Equality Members


        #endregion Equality Members
    }
}