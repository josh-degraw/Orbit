 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 using System.Text;

namespace Orbit.Models
{
    public class PowerSystemData : IAlertableModel
    {
        #region Private Limits
        private int solarRotationUpperLimit = 205;
        private int solarRotationLowerLimit = -205;
        private int solarRotationTolerance = 5;

        private int solarVoltageUpperLimit = 180;
        private int solarVoltageLowerLimit = 0;
        private int solarVoltageTolerance = 10;

        private int batteryTemperatureUpperLimit = 15;
        private int batteryTemperatureLowerLimit = -5;
        private int batteryTemperatureTolerance = 5;

        private int batteryChargeLevelUpperLimit = 105;
        private int batteryChargeLevelLowerLimit = 50;
        private int batteryChargeLevelTolerance = 10;

        private int batterVoltageUpperLimit = 160;
        private int batteryVoltageLowerLimit = 110;
        private int batteryVoltageTolerance = 10;

        #endregion Private Limits

        #region Public Properties

        [NotMapped]
        public string ComponentName => "Power System";
        public DateTimeOffset ReportDateTime { get; private set; } = DateTimeOffset.Now;

        /// <summary>
        /// the positional rotation of the solar array in degrees
        /// </summary>
        [Range(-205, 205)]
        public int SolarArrayRotation { get; set; }

        /// <summary>
        /// voltage output from solar array, will be primary power for station when >= 160v
        /// </summary>
        [Range(0, 180)]
        public int SolarArrayVoltage { get; set; }

        /// <summary>
        /// true when solar array is extended, false when array is retracted
        /// </summary>
        public bool SolarDeployed { get; set; }

        /// <summary>
        /// temperature of the external battery pack
        /// </summary>
        [Range(-115, 130)]
        public double BatteryTemperature { get; set; }

        /// <summary>
        /// charge level as a percentage 0 - 100
        /// </summary>
        [Range(0, 104)]
        public double BatteryChargeLevel { get; set; }

        /// <summary>
        /// current voltage output from batteries
        /// </summary>
        [Range(0, 170)]
        public double BatteryVoltage { get; set; }

        /// <summary>
        /// indicator if battery is in a charging or discharging state 
        /// value is true when solar voltage >= 160v; false otherwise
        /// </summary>
        public bool BatteryIsCharging { get; set; }

        #endregion Public Properties

        #region Check Alerts

        IEnumerable<Alert> IAlertableModel.GenerateAlerts()
        {
            yield break;
        }

        #endregion CheckAlerts

        #region Equality Members

        #region Equality Members
    }
}
