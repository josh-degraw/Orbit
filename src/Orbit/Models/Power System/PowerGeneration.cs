using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

/// <summary>
/// This class is an experiment to see if it would be better to combine the Battery, SolarArray and ShuntUnit classes
/// </summary>


namespace Orbit.Models.Power_System
{
    public class PowerGeneration : IAlertableModel
    {
        [NotMapped]
        public string ComponentName => "Battery";
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

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

        /// <summary>
        /// is true if solar array output is within range and battery is charging, false if otherwise
        /// false state also indicates station is drawing power from batteries
        /// </summary>
        public bool BatteryCharging { get; set; }


        /// <summary>
        /// working range: 126 ~ 173V
        /// Nominal is 160v
        /// </summary>
        [Range(0, 173)]
        public double BatteryOutputVoltage { get; set; }

        [NotMapped]
        public int batteryOutputVoltageUpperLimit = 173;
        [NotMapped]
        public int batteryOutputVoltageLowerLimit = 121;
        [NotMapped]
        public int batteryOutputVoltageTolerance = 5;
        
        /// <summary>
        /// operate to a 35% depth of discharge (dod)  which is opposite of normal charge%, 0% = fully charged, 100% would be fully discharged
        /// 160V expected from charge power source
        /// </summary>
        [Range(0, 105)]
        public int BatteryChargeLevel { get; set; }

        [NotMapped]
        public int batteryChargeUpperLimit = 105;
        [NotMapped]
        public int batteryChargeLowerLimit = 50;
        [NotMapped]
        public int batteryChargeTolerance = 5;

        /// <summary>
        /// Voltage output to station after leaving the DC to DC conversion units
        /// Nominal is ~124V
        /// Acceptable Range: 98 - 136Vdc; transient switching variance of 93 - 141 should return to acceptable range within 5msec
        ///  transient switching values used values for warning ranges
        /// </summary>
        [Range(0, 173)]
        public double VoltageReducerOutput { get; set; }

        [NotMapped]
        public double voltageReducerOutputUpperLimit = 141;
        [NotMapped]
        public double voltageReducerOutputLowerLimit = 93;
        [NotMapped]
        public double voltageReducerOutputTolerance = 5;


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

        private IEnumerable<Alert> CheckBatteryOutputVoltage()
        {
            if (BatteryOutputVoltage > batteryOutputVoltageUpperLimit)
            {
                yield return new Alert(nameof(BatteryOutputVoltage), "Battery voltage has exceeded maximum", AlertLevel.HighError);
            }
            else if (BatteryOutputVoltage >= (batteryOutputVoltageUpperLimit - batteryOutputVoltageTolerance))
            {
                yield return new Alert(nameof(BatteryOutputVoltage), "Battery voltage is high", AlertLevel.HighWarning);
            }
            else if (BatteryOutputVoltage < batteryOutputVoltageUpperLimit)
            {
                yield return new Alert(nameof(BatteryOutputVoltage), "Battery voltage has exceeded minimum", AlertLevel.HighError);
            }
            else if (BatteryOutputVoltage <= (batteryOutputVoltageUpperLimit + batteryOutputVoltageTolerance))
            {
                yield return new Alert(nameof(BatteryOutputVoltage), "Battery voltage is low", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(BatteryOutputVoltage));
            }
        }

        private IEnumerable<Alert> CheckBatteryChargeLevel()
        {
            if (BatteryChargeLevel > batteryChargeUpperLimit)
            {
                yield return new Alert(nameof(BatteryChargeLevel), "Battery charge level has exceeded maximum", AlertLevel.HighError);
            }
            else if (BatteryChargeLevel >= (batteryChargeUpperLimit - batteryChargeTolerance))
            {
                yield return new Alert(nameof(BatteryChargeLevel), "Battery charge level is above threshold", AlertLevel.HighWarning);
            }
            else if (BatteryChargeLevel < batteryChargeLowerLimit)
            {
                yield return new Alert(nameof(BatteryChargeLevel), "Solar array voltage is below minimum", AlertLevel.HighError);
            }
            else if (BatteryChargeLevel <= (batteryChargeLowerLimit + batteryChargeTolerance))
            {
                yield return new Alert(nameof(BatteryChargeLevel), "Battery charge is below threshold", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(BatteryChargeLevel));
            }
        }

        private IEnumerable<Alert> CheckVoltageReducerOutput()
        {
            if (VoltageReducerOutput > voltageReducerOutputUpperLimit)
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output has exceeded maximum allowed voltage", AlertLevel.HighError);
            }
            else if (VoltageReducerOutput >= (voltageReducerOutputUpperLimit - voltageReducerOutputTolerance))
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output is high", AlertLevel.HighWarning);
            }
            else if (VoltageReducerOutput < voltageReducerOutputLowerLimit)
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output has exceeded minimum allowed voltage", AlertLevel.HighError);
            }
            else if (VoltageReducerOutput <= (voltageReducerOutputLowerLimit + voltageReducerOutputTolerance))
            {
                yield return new Alert(nameof(VoltageReducerOutput), "Voltage reducer output is low", AlertLevel.HighWarning);
            }
            else
            {
                yield return Alert.Safe(nameof(VoltageReducerOutput));
            }
        }

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            return CheckSolarArrayRotation().Concat(CheckSolarArrayVoltage()).Concat(CheckBatteryOutputVoltage()).Concat(CheckBatteryChargeLevel()).Concat(CheckVoltageReducerOutput());
        }
    }
}
